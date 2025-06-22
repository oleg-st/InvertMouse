using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using InvertMouse.Utils;

namespace DriverInstaller
{
    public partial class MainForm : Form
    {
        private readonly Utils.DriverInstaller _driverInstaller;
        private const string InstallAction = "/install";
        private const string UninstallAction = "/uninstall";

        private static string GetDriverPath()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directoryName == null)
            {
                return null;
            }

            var architecture = WinAPI.GetArchitecture();
            if (architecture == null)
            {
                return null;
            }

            return Path.Combine(directoryName, "driver", architecture, Utils.DriverInstaller.DriverFileName);
        }

        public MainForm()
        {
            InitializeComponent();
            _driverInstaller = new Utils.DriverInstaller();
        }

        private bool Raise(string action)
        {
            if (!AdminManager.IsAdministrator())
            {
                if (AdminManager.RestartAsAdministrator(action))
                {
                    Close();
                }
                return false;
            }

            return true;
        }

        private void UpdateState()
        {
            var isAdministrator = AdminManager.IsAdministrator();
            if (isAdministrator)
            {
                installBtn.Image = null;
                uninstallBtn.Image = null;
            }

            string state, mark;
            var path = GetDriverPath();
            if (path == null || !File.Exists(path))
            {
                mark = "\u274c";
                state = $"No suitable driver found for {WinAPI.GetArchitecture() ?? "unknown"}";
                installBtn.Enabled = uninstallBtn.Enabled = false;
            }
            else
            {
                var isInstalled = _driverInstaller.IsInstalled();
                var isLoaded = _driverInstaller.IsLoaded();

                installBtn.Enabled = true;
                installBtn.Text = isInstalled ? "Reinstall" : "Install";
                uninstallBtn.Enabled = _driverInstaller.IsInstalledPartially();

                if (isInstalled)
                {
                    state = "Driver is installed";
                    if (isLoaded)
                    {
                        mark = "\u2714";
                        state += " and loaded";
                    }
                    else
                    {
                        mark = "\u231b";
                        state += ", reboot needed to load";
                    }
                }
                else
                {
                    mark = "\u274c";
                    state = "Driver is not installed";
                    if (isLoaded)
                    {
                        mark = "\u231b";
                        state += ", reboot needed to unload";
                    }
                }
            }

            stateLabel.Text = $"{mark} {state}";
        }

        private void DriverForm_Load(object sender, EventArgs e)
        {
            UpdateState();

            var args = Environment.GetCommandLineArgs();
            if (args.Contains(InstallAction))
            {
                DoInstall();
            }
            else if (args.Contains(UninstallAction))
            {
                DoUninstall();
            }
        }

        private void DoInstall()
        {
            if (!Raise(InstallAction))
            {
                return;
            }

            var path = GetDriverPath();
            if (path == null || !File.Exists(path))
            {
                MessageBox.Show("No suitable driver found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                _driverInstaller.Install(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Install failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            UpdateState();
        }

        private void DoUninstall()
        {
            if (!Raise(UninstallAction))
            {
                return;
            }

            try
            {
                _driverInstaller.Uninstall();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uninstall failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            UpdateState();
        }

        private void installBtn_Click(object sender, EventArgs e)
        {
            DoInstall();
        }

        private void uninstallBtn_Click(object sender, EventArgs e)
        {
            DoUninstall();
        }
    }
}
