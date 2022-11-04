using InvertMouse.Inverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CheckState = InvertMouse.Inverter.CheckState;

namespace InvertMouse
{
    public partial class MainForm : Form
    {
        private InvertMouseBase _invertMouse;
        private readonly Dictionary<DriverType, InvertMouseBase> _invertMouseDictionary = new Dictionary<DriverType, InvertMouseBase>();
        private bool XAxisActive => xAxisCB.Checked;
        private bool YAxisActive => yAxisCB.Checked;
        private decimal _yMultiplier = InvertMouseBase.InvertMultiplier, _xMultiplier = InvertMouseBase.InvertMultiplier;

        public MainForm()
        {
            InitializeComponent();
            notifyIcon.Icon = Icon;
            UpdateState();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private InvertMouseBase GetInvertMouse(DriverType driverType)
        {
            if (!_invertMouseDictionary.TryGetValue(driverType, out var invertMouse))
            {
                invertMouse = InvertMouseFactory.Create(driverType);
                _invertMouseDictionary[driverType] = invertMouse;
            }

            return invertMouse;
        }

        private void Detect()
        {
            foreach (DriverType driverType in (DriverType[])Enum.GetValues(typeof(DriverType)))
            {
                var invertMouse = GetInvertMouse(driverType);
                if (invertMouse.State == CheckState.Ok)
                {
                    SetDriver(driverType);
                    Start();
                    break;
                }
            }
        }

        private void Start()
        {
            if (_invertMouse != null)
            {
                _invertMouse.Start();
                UpdateState();
            }
        }

        private void Stop()
        {
            if (_invertMouse != null)
            {
                _invertMouse.Stop();
                UpdateState();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Detect();
            UpdateMultiplierControls();
            ResetAxisMultiplier(xAxisCustomTB);
            ResetAxisMultiplier(yAxisCustomTB);
        }

        private void UpdateState()
        {
            if (_invertMouse == null)
            {
                stateLabel.Text = "Please select driver";
                startStopBtn.Enabled = false;
                return;
            }

            switch (_invertMouse.State)
            {
                case CheckState.LibraryNotLoaded:
                    stateLabel.Text = $"Failed to load library";
                    startStopBtn.Enabled = false;
                    break;
                case CheckState.DriverNotInstalled:
                    stateLabel.Text = $"Please install {_invertMouse.Name} driver";
                    startStopBtn.Enabled = false;
                    break;
                case CheckState.Ok:
                    stateLabel.Text =
                        _invertMouse.IsRunning ? $"Running, delay: {_invertMouse.Delay} ms" : "Ready to run";
                    startStopBtn.Enabled = true;
                    break;
            }

            startStopBtn.Text = _invertMouse.IsRunning ? "Stop" : "Start";
        }

        private void SetOptions()
        {
            if (_invertMouse == null)
            {
                return;
            }

            _invertMouse.WhenCursorIsHidden = cursorHiddenCB.Checked;
            _invertMouse.XMultiplier = XAxisActive ? _xMultiplier : InvertMouseBase.IdentityMultiplier;
            _invertMouse.YMultiplier = YAxisActive ? _yMultiplier : InvertMouseBase.IdentityMultiplier;
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
            UpdateMultiplierControls();
            SetOptions();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && minimizeToTrayCB.Checked)
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

        private void SetDriver(DriverType driverType)
        {
            var running = false;
            if (_invertMouse != null)
            {
                running = _invertMouse.IsRunning;
                _invertMouse?.Stop();
            }

            _invertMouse = GetInvertMouse(driverType);
            if (running)
            {
                _invertMouse.Start();
            }
            SetOptions();
            UpdateState();
            driverComboBox.SelectedIndex = (int)driverType;
        }

        private void driverComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDriver((DriverType)driverComboBox.SelectedIndex);
        }

        private void UpdateMultiplierControls()
        {
            xAxisCustomTB.Enabled = xAxisCustomLabel.Enabled = XAxisActive;
            yAxisCustomTB.Enabled = yAxisCustomLabel.Enabled = YAxisActive;
        }

        private ref decimal GetRefMultiplierForTextBox(TextBox textBox)
        {
            if (textBox == xAxisCustomTB)
            {
                return ref _xMultiplier;
            }

            if (textBox == yAxisCustomTB)
            {
                return ref _yMultiplier;
            }

            throw new ArgumentException("Unknown axis textbox", nameof(textBox));
        }

        private bool TrySetAxisMultiplier(TextBox textBox)
        {
            if (!decimal.TryParse(textBox.Text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture,
                    out var multiplier))
            {
                textBox.ForeColor = Color.Red;
                return false;
            }

            textBox.ForeColor = SystemColors.WindowText;
            GetRefMultiplierForTextBox(textBox) = multiplier;
            UpdateMultiplierControls();
            SetOptions();
            return true;
        }

        private void ResetAxisMultiplier(TextBox textBox)
        {
            var selectStart = textBox.SelectionStart;
            textBox.Text = GetRefMultiplierForTextBox(textBox).ToString(CultureInfo.InvariantCulture);
            textBox.SelectionStart = selectStart;
        }

        private bool SetAxisMultiplier(TextBox textBox)
        {
            if (!TrySetAxisMultiplier(textBox))
            {
                MessageBox.Show($"Invalid multiplier: {textBox.Text}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private void AxisCustomTB_TextChanged(object sender, EventArgs e)
        {
            TrySetAxisMultiplier((TextBox)sender);
        }

        private void AxisCustomTB_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (!SetAxisMultiplier(textBox))
            {
                ResetAxisMultiplier(textBox);
            }
        }

        private void AxisCustomTB_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox) sender;

            if (e.KeyCode == Keys.Enter)
            {
                SetAxisMultiplier(textBox);
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                ResetAxisMultiplier(textBox);
                e.SuppressKeyPress = true;
            }
        }
    }
}
