using InvertMouse.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace InvertMouse
{
    public partial class MainForm : Form
    {
        private readonly DriverInstaller.Utils.DriverInstaller _driverInstaller;

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

            return Path.Combine(directoryName, "driver", architecture, DriverInstaller.Utils.DriverInstaller.DriverFileName);
        }

        public MainForm()
        {
            InitializeComponent();
            _driverInstaller = new DriverInstaller.Utils.DriverInstaller();
        }

        private bool Raise()
        {
            if (!AdminManager.IsAdministrator())
            {
                if (AdminManager.RestartAsAdministrator())
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

            string state;
            var path = GetDriverPath();
            if (path == null || !File.Exists(path))
            {
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
                        state += " and loaded";
                    }
                    else
                    {
                        state += ", reboot needed to load";
                    }
                }
                else
                {
                    state = "Driver is not installed";
                    if (isLoaded)
                    {
                        state += ", reboot needed to unload";
                    }
                }
            }

            stateLabel.Text = state;
        }

        private void DriverForm_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void installBtn_Click(object sender, EventArgs e)
        {
            if (!Raise())
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

        private void uninstallBtn_Click(object sender, EventArgs e)
        {
            if (!Raise())
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
    }
}
