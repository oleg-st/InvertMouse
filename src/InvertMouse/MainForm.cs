using System;
using System.Windows.Forms;

namespace InvertMouse
{
    public partial class MainForm : Form
    {
        private InvertMouse _invertMouse;

        public MainForm()
        {
            InitializeComponent();
            notifyIcon.Icon = Icon;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _invertMouse.Stop();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _invertMouse = new InvertMouse();
            _invertMouse.Start();
            SetOptions();
            UpdateState();
        }
        private void UpdateState()
        {
            switch (_invertMouse.State)
            {
                case InvertMouse.CheckState.LibraryNotLoaded:
                    stateLabel.Text = "Failed to load Interception library";
                    startStopBtn.Enabled = false;
                    break;
                case InvertMouse.CheckState.DriverNotInstalled:
                    stateLabel.Text = "Please install Interception driver";
                    startStopBtn.Enabled = false;
                    break;
                case InvertMouse.CheckState.Ok:
                    stateLabel.Text = _invertMouse.IsRunning ? "Running" : "Ready to run";
                    startStopBtn.Enabled = true;
                    break;
            }

            startStopBtn.Text = _invertMouse.IsRunning ? "Stop" : "Start";
        }

        private void SetOptions()
        {
            _invertMouse.WhenCursorIsHidden = cursorHiddenCB.Checked;
            _invertMouse.XAxis = xAxisCB.Checked;
            _invertMouse.YAxis = yAxisCB.Checked;
        }

        private void StartStopBtn_Click(object sender, EventArgs e)
        {
            if (_invertMouse.IsRunning)
            {
                _invertMouse.Stop();
            }
            else
            {
                _invertMouse.Start();
            }

            UpdateState();
        }

        private void OptionsChanged(object sender, EventArgs e)
        {
            SetOptions();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
        }

        private void Restore()
        {
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == restoreItem)
            {
                Restore();
            }
            else
            {
                Close();
            }
        }

        private void notifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Restore();
            }
        }
    }
}
