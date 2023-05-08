using InvertMouse.Inverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using CheckState = InvertMouse.Inverter.CheckState;

namespace InvertMouse
{
    public partial class MainForm : Form
    {
        private InvertMouseBase _invertMouse;
        private readonly Dictionary<DriverType, InvertMouseBase> _invertMouseDictionary = new Dictionary<DriverType, InvertMouseBase>();

        private bool _autoSaveOptions = true;
        private readonly string _optionsFilename;
        private readonly JsonSerializer _serializer;

        private Options _options;

        public MainForm()
        {
            InitializeComponent();
            _serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            var directoryName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _optionsFilename = Path.Combine(directoryName ?? ".", "Options.json");
            notifyIcon.Icon = Icon;

            if (!LoadOptions())
            {
                ResetOptions();
                SetFromOptions();
            }
        }

        private void OptionsOnChanged(object sender, EventArgs e)
        {
            if (_autoSaveOptions)
            {
                SaveOptions();
            }
        }

        private void SaveOptions()
        {
            try
            {
                using (var streamWriter = new StreamWriter(_optionsFilename))
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    _serializer.Serialize(writer, _options);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot save options to {_optionsFilename}: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                _autoSaveOptions = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _options.Changed -= OptionsOnChanged;

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
            foreach (var driverType in (DriverType[])Enum.GetValues(typeof(DriverType)))
            {
                var invertMouse = GetInvertMouse(driverType);
                if (invertMouse.State == CheckState.Ok)
                {
                    _options.DriverType = driverType;
                    _options.Running = true;
                    break;
                }
            }
        }

        private void Start()
        {
            if (_invertMouse != null)
            {
                _invertMouse.Start();
                _options.Running = _invertMouse.IsRunning;
                UpdateState();
            }
        }

        private void Stop()
        {
            if (_invertMouse != null)
            {
                _invertMouse.Stop();
                _options.Running = _invertMouse.IsRunning;
                UpdateState();
            }
        }

        private bool LoadOptions()
        {
            try
            {
                if (File.Exists(_optionsFilename))
                {
                    using (var streamReader = new StreamReader(_optionsFilename))
                    using (var reader = new JsonTextReader(streamReader))
                    {
                        _options = _serializer.Deserialize<Options>(reader);
                    }

                    SetFromOptions();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot load options from {_optionsFilename}: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            return false;
        }

        private void SetFromOptions()
        {
            SetDriver(_options.DriverType);
            if (_options.Running)
            {
                Start();
            }
            else
            {
                Stop();
            }

            cursorHiddenCB.Checked = _options.WhenCursorIsHidden;
            xAxisCB.Checked = _options.XAxis;
            yAxisCB.Checked = _options.YAxis;
            ResetAxisMultiplier(xAxisCustomTB);
            ResetAxisMultiplier(yAxisCustomTB);
            minimizeToTrayCB.Checked = _options.MinimizeToTray;
        }

        private void ResetOptions()
        {
            _options = new Options();
            Detect();
            SaveOptions();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _options.Changed += OptionsOnChanged;
            UpdateMultiplierControls();
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
                    stateLabel.Text = "Failed to load library";
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

        private void SetInvertMouseOptions()
        {
            if (_invertMouse == null)
            {
                return;
            }

            _invertMouse.WhenCursorIsHidden = _options.WhenCursorIsHidden;
            _invertMouse.XMultiplier = _options.XAxis ? _options.XMultiplier : InvertMouseBase.IdentityMultiplier;
            _invertMouse.YMultiplier = _options.YAxis ? _options.YMultiplier : InvertMouseBase.IdentityMultiplier;
        }

        private void StartStopBtn_Click(object sender, EventArgs e)
        {
            if (_invertMouse.IsRunning)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && _options.MinimizeToTray)
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
            SetInvertMouseOptions();
            if (running)
            {
                _invertMouse.Start();
            }

            _options.DriverType = driverType;
            UpdateState();
            driverComboBox.SelectedIndex = (int)driverType;
        }

        private void driverComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDriver((DriverType)driverComboBox.SelectedIndex);
        }

        private void UpdateMultiplierControls()
        {
            xAxisCustomTB.Enabled = xAxisCustomLabel.Enabled = _options.XAxis;
            yAxisCustomTB.Enabled = yAxisCustomLabel.Enabled = _options.YAxis;
        }

        private decimal GetMultiplierForTextBox(TextBox textBox)
        {
            if (textBox == xAxisCustomTB)
            {
                return _options.XMultiplier;
            }

            if (textBox == yAxisCustomTB)
            {
                return _options.YMultiplier;
            }

            throw new ArgumentException("Unknown axis text box", nameof(textBox));
        }

        private void SetMultiplierForTextBox(TextBox textBox, decimal value)
        {
            if (textBox == xAxisCustomTB)
            {
                _options.XMultiplier = value;
            } else if (textBox == yAxisCustomTB)
            {
                _options.YMultiplier = value;
            }
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
            SetMultiplierForTextBox(textBox, multiplier);
            UpdateMultiplierControls();
            SetInvertMouseOptions();
            return true;
        }

        private void ResetAxisMultiplier(TextBox textBox)
        {
            var selectStart = textBox.SelectionStart;
            textBox.Text = GetMultiplierForTextBox(textBox).ToString(CultureInfo.InvariantCulture);
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

        private void cursorHiddenCB_CheckedChanged(object sender, EventArgs e)
        {
            _options.WhenCursorIsHidden = cursorHiddenCB.Checked;
            SetInvertMouseOptions();
        }

        private void yAxisCB_CheckedChanged(object sender, EventArgs e)
        {
            _options.YAxis = yAxisCB.Checked;
            UpdateMultiplierControls();
            SetInvertMouseOptions();
        }

        private void xAxisCB_CheckedChanged(object sender, EventArgs e)
        {
            _options.XAxis = xAxisCB.Checked;
            UpdateMultiplierControls();
            SetInvertMouseOptions();
        }

        private void minimizeToTrayCB_CheckedChanged(object sender, EventArgs e)
        {
            _options.MinimizeToTray = minimizeToTrayCB.Checked;
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
