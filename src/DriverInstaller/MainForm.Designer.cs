
namespace DriverInstaller
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
            this.stateLabel = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installBtn = new System.Windows.Forms.Button();
            this.uninstallBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // stateLabel
            // 
            this.stateLabel.Location = new System.Drawing.Point(14, 9);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(287, 53);
            this.stateLabel.TabIndex = 1;
            this.stateLabel.Text = "StateLabel";
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Text = "InvertMouse";
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreItem,
            this.closeItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(114, 48);
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
            // installBtn
            // 
            this.installBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.installBtn.Image = ((System.Drawing.Image)(resources.GetObject("installBtn.Image")));
            this.installBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.installBtn.Location = new System.Drawing.Point(18, 71);
            this.installBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.installBtn.Name = "installBtn";
            this.installBtn.Size = new System.Drawing.Size(280, 35);
            this.installBtn.TabIndex = 14;
            this.installBtn.Text = "Install";
            this.installBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.installBtn.UseVisualStyleBackColor = true;
            this.installBtn.Click += new System.EventHandler(this.installBtn_Click);
            // 
            // uninstallBtn
            // 
            this.uninstallBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uninstallBtn.Image = ((System.Drawing.Image)(resources.GetObject("uninstallBtn.Image")));
            this.uninstallBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uninstallBtn.Location = new System.Drawing.Point(18, 116);
            this.uninstallBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uninstallBtn.Name = "uninstallBtn";
            this.uninstallBtn.Size = new System.Drawing.Size(280, 35);
            this.uninstallBtn.TabIndex = 15;
            this.uninstallBtn.Text = "Uninstall";
            this.uninstallBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.uninstallBtn.UseVisualStyleBackColor = true;
            this.uninstallBtn.Click += new System.EventHandler(this.uninstallBtn_Click);
            // 
            // DriverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 163);
            this.Controls.Add(this.uninstallBtn);
            this.Controls.Add(this.installBtn);
            this.Controls.Add(this.stateLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "DriverForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InvertMouseDriver";
            this.Load += new System.EventHandler(this.DriverForm_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem restoreItem;
        private System.Windows.Forms.ToolStripMenuItem closeItem;
        private System.Windows.Forms.Button installBtn;
        private System.Windows.Forms.Button uninstallBtn;
    }
}

