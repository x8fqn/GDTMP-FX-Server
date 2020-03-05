namespace Darkly.GDTMP
{
    partial class SettingsForm
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
            this.gb1 = new System.Windows.Forms.GroupBox();
            this.chkSSS = new System.Windows.Forms.CheckBox();
            this.cbMM = new System.Windows.Forms.ComboBox();
            this.lblMM = new System.Windows.Forms.Label();
            this.lblRestart = new System.Windows.Forms.Label();
            this.chkSB = new System.Windows.Forms.CheckBox();
            this.chkPOI = new System.Windows.Forms.CheckBox();
            this.chkEOP = new System.Windows.Forms.CheckBox();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.nudTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblPort = new System.Windows.Forms.Label();
            this.chkAPF = new System.Windows.Forms.CheckBox();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.chkCMA = new System.Windows.Forms.CheckBox();
            this.gb2 = new System.Windows.Forms.GroupBox();
            this.tbDesc = new System.Windows.Forms.TextBox();
            this.lbDesc = new System.Windows.Forms.Label();
            this.chkDesc = new System.Windows.Forms.CheckBox();
            this.chkRB = new System.Windows.Forms.CheckBox();
            this.chkTS = new System.Windows.Forms.CheckBox();
            this.chkSC = new System.Windows.Forms.CheckBox();
            this.chkOC = new System.Windows.Forms.CheckBox();
            this.tbMOTD = new System.Windows.Forms.TextBox();
            this.lblMOTD = new System.Windows.Forms.Label();
            this.chkMOTD = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.gb1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.gb2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb1
            // 
            this.gb1.Controls.Add(this.chkSSS);
            this.gb1.Controls.Add(this.cbMM);
            this.gb1.Controls.Add(this.lblMM);
            this.gb1.Controls.Add(this.lblRestart);
            this.gb1.Controls.Add(this.chkSB);
            this.gb1.Controls.Add(this.chkPOI);
            this.gb1.Controls.Add(this.chkEOP);
            this.gb1.Controls.Add(this.lblTimeout);
            this.gb1.Controls.Add(this.nudTimeout);
            this.gb1.Controls.Add(this.lblPort);
            this.gb1.Controls.Add(this.chkAPF);
            this.gb1.Controls.Add(this.nudPort);
            this.gb1.Controls.Add(this.chkCMA);
            this.gb1.Location = new System.Drawing.Point(12, 12);
            this.gb1.Name = "gb1";
            this.gb1.Size = new System.Drawing.Size(216, 240);
            this.gb1.TabIndex = 0;
            this.gb1.TabStop = false;
            this.gb1.Text = "Players and connections";
            // 
            // chkSSS
            // 
            this.chkSSS.AutoSize = true;
            this.chkSSS.Location = new System.Drawing.Point(6, 212);
            this.chkSSS.Name = "chkSSS";
            this.chkSSS.Size = new System.Drawing.Size(180, 17);
            this.chkSSS.TabIndex = 8;
            this.chkSSS.Text = "Enable forced server-side saving";
            this.chkSSS.UseVisualStyleBackColor = true;
            // 
            // cbMM
            // 
            this.cbMM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMM.FormattingEnabled = true;
            this.cbMM.Items.AddRange(new object[] {
            "Off",
            "1T",
            "10T",
            "100T",
            "1Q"});
            this.cbMM.Location = new System.Drawing.Point(156, 70);
            this.cbMM.Name = "cbMM";
            this.cbMM.Size = new System.Drawing.Size(54, 21);
            this.cbMM.TabIndex = 2;
            // 
            // lblMM
            // 
            this.lblMM.AutoSize = true;
            this.lblMM.Location = new System.Drawing.Point(6, 73);
            this.lblMM.Name = "lblMM";
            this.lblMM.Size = new System.Drawing.Size(144, 13);
            this.lblMM.TabIndex = 16;
            this.lblMM.Text = "Kick players if cash exceeds:";
            // 
            // lblRestart
            // 
            this.lblRestart.AutoSize = true;
            this.lblRestart.Location = new System.Drawing.Point(117, 21);
            this.lblRestart.Name = "lblRestart";
            this.lblRestart.Size = new System.Drawing.Size(82, 13);
            this.lblRestart.TabIndex = 15;
            this.lblRestart.Text = "(requires restart)";
            // 
            // chkSB
            // 
            this.chkSB.AutoSize = true;
            this.chkSB.Location = new System.Drawing.Point(6, 189);
            this.chkSB.Name = "chkSB";
            this.chkSB.Size = new System.Drawing.Size(200, 17);
            this.chkSB.TabIndex = 7;
            this.chkSB.Text = "Make server visible in server browser";
            this.chkSB.UseVisualStyleBackColor = true;
            // 
            // chkPOI
            // 
            this.chkPOI.AutoSize = true;
            this.chkPOI.Location = new System.Drawing.Point(6, 166);
            this.chkPOI.Name = "chkPOI";
            this.chkPOI.Size = new System.Drawing.Size(152, 17);
            this.chkPOI.TabIndex = 6;
            this.chkPOI.Text = "Remember ops by their IPs";
            this.chkPOI.UseVisualStyleBackColor = true;
            // 
            // chkEOP
            // 
            this.chkEOP.AutoSize = true;
            this.chkEOP.Location = new System.Drawing.Point(6, 143);
            this.chkEOP.Name = "chkEOP";
            this.chkEOP.Size = new System.Drawing.Size(131, 17);
            this.chkEOP.TabIndex = 5;
            this.chkEOP.Text = "Give ops full privileges";
            this.chkEOP.UseVisualStyleBackColor = true;
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(6, 47);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(108, 13);
            this.lblTimeout.TabIndex = 5;
            this.lblTimeout.Text = "Timeout (in seconds):";
            // 
            // nudTimeout
            // 
            this.nudTimeout.Location = new System.Drawing.Point(120, 45);
            this.nudTimeout.Maximum = new decimal(new int[] {
            15000000,
            0,
            0,
            0});
            this.nudTimeout.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTimeout.Name = "nudTimeout";
            this.nudTimeout.Size = new System.Drawing.Size(70, 20);
            this.nudTimeout.TabIndex = 1;
            this.nudTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 21);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port:";
            // 
            // chkAPF
            // 
            this.chkAPF.AutoSize = true;
            this.chkAPF.Location = new System.Drawing.Point(6, 97);
            this.chkAPF.Name = "chkAPF";
            this.chkAPF.Size = new System.Drawing.Size(146, 17);
            this.chkAPF.TabIndex = 3;
            this.chkAPF.Text = "Automatic port forwarding";
            this.chkAPF.UseVisualStyleBackColor = true;
            // 
            // nudPort
            // 
            this.nudPort.Location = new System.Drawing.Point(41, 19);
            this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(70, 20);
            this.nudPort.TabIndex = 0;
            this.nudPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkCMA
            // 
            this.chkCMA.AutoSize = true;
            this.chkCMA.Location = new System.Drawing.Point(6, 120);
            this.chkCMA.Name = "chkCMA";
            this.chkCMA.Size = new System.Drawing.Size(95, 17);
            this.chkCMA.TabIndex = 4;
            this.chkCMA.Text = "Allow cheating";
            this.chkCMA.UseVisualStyleBackColor = true;
            // 
            // gb2
            // 
            this.gb2.Controls.Add(this.tbDesc);
            this.gb2.Controls.Add(this.lbDesc);
            this.gb2.Controls.Add(this.chkDesc);
            this.gb2.Controls.Add(this.chkRB);
            this.gb2.Controls.Add(this.chkTS);
            this.gb2.Controls.Add(this.chkSC);
            this.gb2.Controls.Add(this.chkOC);
            this.gb2.Controls.Add(this.tbMOTD);
            this.gb2.Controls.Add(this.lblMOTD);
            this.gb2.Controls.Add(this.chkMOTD);
            this.gb2.Location = new System.Drawing.Point(234, 12);
            this.gb2.Name = "gb2";
            this.gb2.Size = new System.Drawing.Size(372, 240);
            this.gb2.TabIndex = 1;
            this.gb2.TabStop = false;
            this.gb2.Text = "Gameplay and messages";
            // 
            // tbDesc
            // 
            this.tbDesc.Location = new System.Drawing.Point(72, 87);
            this.tbDesc.MaxLength = 40;
            this.tbDesc.Name = "tbDesc";
            this.tbDesc.Size = new System.Drawing.Size(294, 20);
            this.tbDesc.TabIndex = 12;
            // 
            // lbDesc
            // 
            this.lbDesc.AutoSize = true;
            this.lbDesc.Location = new System.Drawing.Point(6, 90);
            this.lbDesc.Name = "lbDesc";
            this.lbDesc.Size = new System.Drawing.Size(60, 13);
            this.lbDesc.TabIndex = 15;
            this.lbDesc.Text = "Desciption:";
            // 
            // chkDesc
            // 
            this.chkDesc.AutoSize = true;
            this.chkDesc.Location = new System.Drawing.Point(6, 66);
            this.chkDesc.Name = "chkDesc";
            this.chkDesc.Size = new System.Drawing.Size(113, 17);
            this.chkDesc.TabIndex = 11;
            this.chkDesc.Text = "Enable description";
            this.chkDesc.UseVisualStyleBackColor = true;
            this.chkDesc.CheckedChanged += new System.EventHandler(this.chkDesc_CheckedChanged);
            // 
            // chkRB
            // 
            this.chkRB.AutoSize = true;
            this.chkRB.Location = new System.Drawing.Point(9, 212);
            this.chkRB.Name = "chkRB";
            this.chkRB.Size = new System.Drawing.Size(326, 17);
            this.chkRB.TabIndex = 16;
            this.chkRB.Text = "Allow players to compete with review scores (requires time sync)";
            this.chkRB.UseVisualStyleBackColor = true;
            // 
            // chkTS
            // 
            this.chkTS.AutoSize = true;
            this.chkTS.Location = new System.Drawing.Point(9, 189);
            this.chkTS.Name = "chkTS";
            this.chkTS.Size = new System.Drawing.Size(257, 17);
            this.chkTS.TabIndex = 15;
            this.chkTS.Text = "Enable time synchronization (starts at Y1 M6 W1)";
            this.chkTS.UseVisualStyleBackColor = true;
            this.chkTS.CheckedChanged += new System.EventHandler(this.chkTS_CheckedChanged);
            // 
            // chkSC
            // 
            this.chkSC.AutoSize = true;
            this.chkSC.Location = new System.Drawing.Point(9, 166);
            this.chkSC.Name = "chkSC";
            this.chkSC.Size = new System.Drawing.Size(358, 17);
            this.chkSC.TabIndex = 14;
            this.chkSC.Text = "Disallow other players\' consoles to be used before original release date";
            this.chkSC.UseVisualStyleBackColor = true;
            // 
            // chkOC
            // 
            this.chkOC.AutoSize = true;
            this.chkOC.Location = new System.Drawing.Point(9, 143);
            this.chkOC.Name = "chkOC";
            this.chkOC.Size = new System.Drawing.Size(358, 17);
            this.chkOC.TabIndex = 13;
            this.chkOC.Text = "Allow other players\' consoles to be used even if the developer is offline";
            this.chkOC.UseVisualStyleBackColor = true;
            // 
            // tbMOTD
            // 
            this.tbMOTD.Location = new System.Drawing.Point(54, 40);
            this.tbMOTD.MaxLength = 80;
            this.tbMOTD.Name = "tbMOTD";
            this.tbMOTD.Size = new System.Drawing.Size(312, 20);
            this.tbMOTD.TabIndex = 10;
            // 
            // lblMOTD
            // 
            this.lblMOTD.AutoSize = true;
            this.lblMOTD.Location = new System.Drawing.Point(6, 43);
            this.lblMOTD.Name = "lblMOTD";
            this.lblMOTD.Size = new System.Drawing.Size(42, 13);
            this.lblMOTD.TabIndex = 8;
            this.lblMOTD.Text = "MOTD:";
            // 
            // chkMOTD
            // 
            this.chkMOTD.AutoSize = true;
            this.chkMOTD.Location = new System.Drawing.Point(6, 17);
            this.chkMOTD.Name = "chkMOTD";
            this.chkMOTD.Size = new System.Drawing.Size(94, 17);
            this.chkMOTD.TabIndex = 9;
            this.chkMOTD.Text = "Enable MOTD";
            this.chkMOTD.UseVisualStyleBackColor = true;
            this.chkMOTD.CheckedChanged += new System.EventHandler(this.chkMOTD_CheckedChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(314, 258);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(292, 23);
            this.cancelButton.TabIndex = 18;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 258);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(292, 23);
            this.saveButton.TabIndex = 17;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 293);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.gb2);
            this.Controls.Add(this.gb1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(634, 327);
            this.MinimumSize = new System.Drawing.Size(634, 327);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.gb1.ResumeLayout(false);
            this.gb1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.gb2.ResumeLayout(false);
            this.gb2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb1;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.CheckBox chkCMA;
        private System.Windows.Forms.GroupBox gb2;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.CheckBox chkAPF;
        private System.Windows.Forms.CheckBox chkPOI;
        private System.Windows.Forms.CheckBox chkEOP;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.NumericUpDown nudTimeout;
        private System.Windows.Forms.CheckBox chkSC;
        private System.Windows.Forms.CheckBox chkOC;
        private System.Windows.Forms.TextBox tbMOTD;
        private System.Windows.Forms.Label lblMOTD;
        private System.Windows.Forms.CheckBox chkMOTD;
        private System.Windows.Forms.CheckBox chkTS;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox chkRB;
        private System.Windows.Forms.CheckBox chkSB;
        private System.Windows.Forms.Label lblRestart;
        private System.Windows.Forms.TextBox tbDesc;
        private System.Windows.Forms.Label lbDesc;
        private System.Windows.Forms.CheckBox chkDesc;
        private System.Windows.Forms.ComboBox cbMM;
        private System.Windows.Forms.Label lblMM;
        private System.Windows.Forms.CheckBox chkSSS;
    }
}