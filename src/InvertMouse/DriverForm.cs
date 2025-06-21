using InvertMouse.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace InvertMouse
{
    public partial class DriverForm : Form
    {
        private readonly DriverInstaller _driverInstaller;

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

            return Path.Combine(directoryName, "driver", architecture, DriverInstaller.DriverFileName);
        }

        public DriverForm()
        {
            InitializeComponent();
            _driverInstaller = new DriverInstaller();
            DialogResult = DialogResult.OK;
        }

        private bool Raise()
        {
            if (!AdminManager.IsAdministrator())
            {
                DialogResult = DialogResult.Abort;
                Close();
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

            var isInstalled = _driverInstaller.IsInstalled();
            var isLoaded = _driverInstaller.IsLoaded();

            installBtn.Enabled = !isInstalled;
            uninstallBtn.Enabled = isInstalled;

            string state;
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
