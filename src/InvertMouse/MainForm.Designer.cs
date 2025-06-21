
namespace InvertMouse
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.startStopBtn = new System.Windows.Forms.Button();
            this.stateLabel = new System.Windows.Forms.Label();
            this.cursorHiddenCB = new System.Windows.Forms.CheckBox();
            this.yAxisCB = new System.Windows.Forms.CheckBox();
            this.xAxisCB = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.driverLabel = new System.Windows.Forms.Label();
            this.driverComboBox = new System.Windows.Forms.ComboBox();
            this.minimizeToTrayCB = new System.Windows.Forms.CheckBox();
            this.yAxisCustomTB = new System.Windows.Forms.TextBox();
            this.yAxisCustomLabel = new System.Windows.Forms.Label();
            this.xAxisCustomLabel = new System.Windows.Forms.Label();
            this.xAxisCustomTB = new System.Windows.Forms.TextBox();
            this.startMinimizedCB = new System.Windows.Forms.CheckBox();
            this.startStopByKeyCB = new System.Windows.Forms.CheckBox();
            this.startStopKeyTB = new System.Windows.Forms.TextBox();
            this.shieldIconPB = new System.Windows.Forms.PictureBox();
            this.invertMouseDriverBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shieldIconPB)).BeginInit();
            this.SuspendLayout();
            // 
            // startStopBtn
            // 
            this.startStopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startStopBtn.Location = new System.Drawing.Point(13, 100);
            this.startStopBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startStopBtn.Name = "startStopBtn";
            this.startStopBtn.Size = new System.Drawing.Size(285, 57);
            this.startStopBtn.TabIndex = 2;
            this.startStopBtn.Text = "Start";
            this.startStopBtn.UseVisualStyleBackColor = true;
            this.startStopBtn.Click += new System.EventHandler(this.StartStopBtn_Click);
            // 
            // stateLabel
            // 
            this.stateLabel.Location = new System.Drawing.Point(12, 74);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(287, 21);
            this.stateLabel.TabIndex = 1;
            this.stateLabel.Text = "StateLabel";
            // 
            // cursorHiddenCB
            // 
            this.cursorHiddenCB.AutoSize = true;
            this.cursorHiddenCB.Checked = true;
            this.cursorHiddenCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cursorHiddenCB.Location = new System.Drawing.Point(16, 172);
            this.cursorHiddenCB.Name = "cursorHiddenCB";
            this.cursorHiddenCB.Size = new System.Drawing.Size(185, 25);
            this.cursorHiddenCB.TabIndex = 3;
            this.cursorHiddenCB.Text = "When cursor is hidden";
            this.cursorHiddenCB.UseVisualStyleBackColor = true;
            this.cursorHiddenCB.CheckedChanged += new System.EventHandler(this.cursorHiddenCB_CheckedChanged);
            // 
            // yAxisCB
            // 
            this.yAxisCB.AutoSize = true;
            this.yAxisCB.Checked = true;
            this.yAxisCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.yAxisCB.Location = new System.Drawing.Point(16, 203);
            this.yAxisCB.Name = "yAxisCB";
            this.yAxisCB.Size = new System.Drawing.Size(70, 25);
            this.yAxisCB.TabIndex = 4;
            this.yAxisCB.Text = "Y Axis";
            this.yAxisCB.UseVisualStyleBackColor = true;
            this.yAxisCB.CheckedChanged += new System.EventHandler(this.yAxisCB_CheckedChanged);
            // 
            // xAxisCB
            // 
            this.xAxisCB.AutoSize = true;
            this.xAxisCB.Location = new System.Drawing.Point(16, 234);
            this.xAxisCB.Name = "xAxisCB";
            this.xAxisCB.Size = new System.Drawing.Size(70, 25);
            this.xAxisCB.TabIndex = 6;
            this.xAxisCB.Text = "X Axis";
            this.xAxisCB.UseVisualStyleBackColor = true;
            this.xAxisCB.CheckedChanged += new System.EventHandler(this.xAxisCB_CheckedChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Text = "InvertMouse";
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreItem,
            this.closeItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(114, 48);
            this.contextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_ItemClicked);
            // 
            // restoreItem
            // 
            this.restoreItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.restoreItem.Name = "restoreItem";
            this.restoreItem.Size = new System.Drawing.Size(113, 22);
            this.restoreItem.Text = "Restore";
            // 
            // closeItem
            // 
            this.closeItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.closeItem.Name = "closeItem";
            this.closeItem.Size = new System.Drawing.Size(113, 22);
            this.closeItem.Text = "Close";
            // 
            // driverLabel
            // 
            this.driverLabel.Location = new System.Drawing.Point(12, 9);
            this.driverLabel.Name = "driverLabel";
            this.driverLabel.Size = new System.Drawing.Size(287, 21);
            this.driverLabel.TabIndex = 5;
            this.driverLabel.Text = "Driver";
            // 
            // driverComboBox
            // 
            this.driverComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.driverComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driverComboBox.FormattingEnabled = true;
            this.driverComboBox.Items.AddRange(new object[] {
            "Interception",
            "RawAccel",
            "InvertMouse"});
            this.driverComboBox.Location = new System.Drawing.Point(16, 33);
            this.driverComboBox.Name = "driverComboBox";
            this.driverComboBox.Size = new System.Drawing.Size(282, 29);
            this.driverComboBox.TabIndex = 1;
            this.driverComboBox.SelectedIndexChanged += new System.EventHandler(this.driverComboBox_SelectedIndexChanged);
            // 
            // minimizeToTrayCB
            // 
            this.minimizeToTrayCB.AutoSize = true;
            this.minimizeToTrayCB.Location = new System.Drawing.Point(16, 265);
            this.minimizeToTrayCB.Name = "minimizeToTrayCB";
            this.minimizeToTrayCB.Size = new System.Drawing.Size(142, 25);
            this.minimizeToTrayCB.TabIndex = 8;
            this.minimizeToTrayCB.Text = "Minimize to tray";
            this.minimizeToTrayCB.UseVisualStyleBackColor = true;
            this.minimizeToTrayCB.CheckedChanged += new System.EventHandler(this.minimizeToTrayCB_CheckedChanged);
            // 
            // yAxisCustomTB
            // 
            this.yAxisCustomTB.Location = new System.Drawing.Point(176, 200);
            this.yAxisCustomTB.Name = "yAxisCustomTB";
            this.yAxisCustomTB.Size = new System.Drawing.Size(55, 29);
            this.yAxisCustomTB.TabIndex = 5;
            this.yAxisCustomTB.TextChanged += new System.EventHandler(this.AxisCustomTB_TextChanged);
            this.yAxisCustomTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AxisCustomTB_KeyDown);
            this.yAxisCustomTB.Leave += new System.EventHandler(this.AxisCustomTB_Leave);
            // 
            // yAxisCustomLabel
            // 
            this.yAxisCustomLabel.Location = new System.Drawing.Point(92, 204);
            this.yAxisCustomLabel.Name = "yAxisCustomLabel";
            this.yAxisCustomLabel.Size = new System.Drawing.Size(81, 21);
            this.yAxisCustomLabel.TabIndex = 5;
            this.yAxisCustomLabel.Text = "Multiplier ";
            // 
            // xAxisCustomLabel
            // 
            this.xAxisCustomLabel.Location = new System.Drawing.Point(92, 235);
            this.xAxisCustomLabel.Name = "xAxisCustomLabel";
            this.xAxisCustomLabel.Size = new System.Drawing.Size(81, 21);
            this.xAxisCustomLabel.TabIndex = 7;
            this.xAxisCustomLabel.Text = "Multiplier ";
            // 
            // xAxisCustomTB
            // 
            this.xAxisCustomTB.Location = new System.Drawing.Point(176, 231);
            this.xAxisCustomTB.Name = "xAxisCustomTB";
            this.xAxisCustomTB.Size = new System.Drawing.Size(55, 29);
            this.xAxisCustomTB.TabIndex = 7;
            this.xAxisCustomTB.TextChanged += new System.EventHandler(this.AxisCustomTB_TextChanged);
            this.xAxisCustomTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AxisCustomTB_KeyDown);
            // 
            // startMinimizedCB
            // 
            this.startMinimizedCB.AutoSize = true;
            this.startMinimizedCB.Location = new System.Drawing.Point(164, 265);
            this.startMinimizedCB.Name = "startMinimizedCB";
            this.startMinimizedCB.Size = new System.Drawing.Size(138, 25);
            this.startMinimizedCB.TabIndex = 9;
            this.startMinimizedCB.Text = "Start minimized";
            this.startMinimizedCB.UseVisualStyleBackColor = true;
            this.startMinimizedCB.CheckedChanged += new System.EventHandler(this.startMinimizedCB_CheckedChanged);
            // 
            // startStopByKeyCB
            // 
            this.startStopByKeyCB.AutoSize = true;
            this.startStopByKeyCB.Location = new System.Drawing.Point(16, 296);
            this.startStopByKeyCB.Name = "startStopByKeyCB";
            this.startStopByKeyCB.Size = new System.Drawing.Size(146, 25);
            this.startStopByKeyCB.TabIndex = 11;
            this.startStopByKeyCB.Text = "Start/stop by key";
            this.startStopByKeyCB.UseVisualStyleBackColor = true;
            this.startStopByKeyCB.CheckedChanged += new System.EventHandler(this.toggleCB_CheckedChanged);
            // 
            // startStopKeyTB
            // 
            this.startStopKeyTB.BackColor = System.Drawing.SystemColors.Window;
            this.startStopKeyTB.Location = new System.Drawing.Point(16, 327);
            this.startStopKeyTB.Name = "startStopKeyTB";
            this.startStopKeyTB.ReadOnly = true;
            this.startStopKeyTB.ShortcutsEnabled = false;
            this.startStopKeyTB.Size = new System.Drawing.Size(282, 29);
            this.startStopKeyTB.TabIndex = 12;
            this.startStopKeyTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyTB_KeyDown);
            this.startStopKeyTB.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyTB_KeyUp);
            this.startStopKeyTB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.keyTB_MouseDown);
            this.startStopKeyTB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.keyTB_MouseUp);
            this.startStopKeyTB.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.keyTB_PreviewKeyDown);
            // 
            // shieldIconPB
            // 
            this.shieldIconPB.Image = ((System.Drawing.Image)(resources.GetObject("shieldIconPB.Image")));
            this.shieldIconPB.Location = new System.Drawing.Point(161, 301);
            this.shieldIconPB.Name = "shieldIconPB";
            this.shieldIconPB.Size = new System.Drawing.Size(16, 16);
            this.shieldIconPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.shieldIconPB.TabIndex = 13;
            this.shieldIconPB.TabStop = false;
            this.shieldIconPB.Visible = false;
            this.shieldIconPB.WaitOnLoad = true;
            // 
            // invertMouseDriverBtn
            // 
            this.invertMouseDriverBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.invertMouseDriverBtn.Location = new System.Drawing.Point(16, 364);
            this.invertMouseDriverBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.invertMouseDriverBtn.Name = "invertMouseDriverBtn";
            this.invertMouseDriverBtn.Size = new System.Drawing.Size(285, 35);
            this.invertMouseDriverBtn.TabIndex = 14;
            this.invertMouseDriverBtn.Text = "InvertMouseDriver";
            this.invertMouseDriverBtn.UseVisualStyleBackColor = true;
            this.invertMouseDriverBtn.Click += new System.EventHandler(this.invertMouseDriverBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 407);
            this.Controls.Add(this.invertMouseDriverBtn);
            this.Controls.Add(this.shieldIconPB);
            this.Controls.Add(this.startStopKeyTB);
            this.Controls.Add(this.startStopByKeyCB);
            this.Controls.Add(this.startMinimizedCB);
            this.Controls.Add(this.xAxisCustomLabel);
            this.Controls.Add(this.xAxisCustomTB);
            this.Controls.Add(this.yAxisCustomLabel);
            this.Controls.Add(this.yAxisCustomTB);
            this.Controls.Add(this.minimizeToTrayCB);
            this.Controls.Add(this.driverComboBox);
            this.Controls.Add(this.driverLabel);
            this.Controls.Add(this.xAxisCB);
            this.Controls.Add(this.yAxisCB);
            this.Controls.Add(this.cursorHiddenCB);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.startStopBtn);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InvertMouse";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.shieldIconPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startStopBtn;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.CheckBox cursorHiddenCB;
        private System.Windows.Forms.CheckBox yAxisCB;
        private System.Windows.Forms.CheckBox xAxisCB;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem restoreItem;
        private System.Windows.Forms.ToolStripMenuItem closeItem;
        private System.Windows.Forms.Label driverLabel;
        private System.Windows.Forms.ComboBox driverComboBox;
        private System.Windows.Forms.CheckBox minimizeToTrayCB;
        private System.Windows.Forms.TextBox yAxisCustomTB;
        private System.Windows.Forms.Label yAxisCustomLabel;
        private System.Windows.Forms.Label xAxisCustomLabel;
        private System.Windows.Forms.TextBox xAxisCustomTB;
        private System.Windows.Forms.CheckBox startMinimizedCB;
        private System.Windows.Forms.CheckBox startStopByKeyCB;
        private System.Windows.Forms.TextBox startStopKeyTB;
        private System.Windows.Forms.PictureBox shieldIconPB;
        private System.Windows.Forms.Button invertMouseDriverBtn;
    }
}

