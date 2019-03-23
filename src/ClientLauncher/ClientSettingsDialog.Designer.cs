namespace MUnique.OpenMU.ClientLauncher
{
    partial class ClientSettingsDialog
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
            this.windowModeCheckBox = new System.Windows.Forms.CheckBox();
            this.musicActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.soundActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.soundVolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.clientLanguageComboBox = new System.Windows.Forms.ComboBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.clientResolutionComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.colorDepthComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeTrackBar)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // windowModeCheckBox
            // 
            this.windowModeCheckBox.AutoSize = true;
            this.windowModeCheckBox.Location = new System.Drawing.Point(19, 248);
            this.windowModeCheckBox.Name = "windowModeCheckBox";
            this.windowModeCheckBox.Size = new System.Drawing.Size(95, 17);
            this.windowModeCheckBox.TabIndex = 0;
            this.windowModeCheckBox.Text = "Window Mode";
            this.windowModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // musicActiveCheckBox
            // 
            this.musicActiveCheckBox.AutoSize = true;
            this.musicActiveCheckBox.Location = new System.Drawing.Point(117, 19);
            this.musicActiveCheckBox.Name = "musicActiveCheckBox";
            this.musicActiveCheckBox.Size = new System.Drawing.Size(115, 17);
            this.musicActiveCheckBox.TabIndex = 3;
            this.musicActiveCheckBox.Text = "Background Music";
            this.musicActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // soundActiveCheckBox
            // 
            this.soundActiveCheckBox.AutoSize = true;
            this.soundActiveCheckBox.Location = new System.Drawing.Point(9, 19);
            this.soundActiveCheckBox.Name = "soundActiveCheckBox";
            this.soundActiveCheckBox.Size = new System.Drawing.Size(93, 17);
            this.soundActiveCheckBox.TabIndex = 4;
            this.soundActiveCheckBox.Text = "Sound Effects";
            this.soundActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.soundVolumeTrackBar);
            this.groupBox3.Controls.Add(this.musicActiveCheckBox);
            this.groupBox3.Controls.Add(this.soundActiveCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(16, 100);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(270, 96);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sound";
            // 
            // soundVolumeTrackBar
            // 
            this.soundVolumeTrackBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.soundVolumeTrackBar.Location = new System.Drawing.Point(3, 48);
            this.soundVolumeTrackBar.Maximum = 9;
            this.soundVolumeTrackBar.Name = "soundVolumeTrackBar";
            this.soundVolumeTrackBar.Size = new System.Drawing.Size(264, 45);
            this.soundVolumeTrackBar.TabIndex = 5;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.clientLanguageComboBox);
            this.groupBox4.Location = new System.Drawing.Point(16, 202);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(270, 40);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Language";
            // 
            // clientLanguageComboBox
            // 
            this.clientLanguageComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.clientLanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clientLanguageComboBox.FormattingEnabled = true;
            this.clientLanguageComboBox.Items.AddRange(new object[] {
            "English",
            "Portuguese",
            "Spanish"});
            this.clientLanguageComboBox.Location = new System.Drawing.Point(3, 16);
            this.clientLanguageComboBox.Name = "clientLanguageComboBox";
            this.clientLanguageComboBox.Size = new System.Drawing.Size(264, 21);
            this.clientLanguageComboBox.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(131, 278);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonClick);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(212, 278);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "Cancel";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.clientResolutionComboBox);
            this.groupBox5.Location = new System.Drawing.Point(16, 11);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(270, 39);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Screen Resolution";
            // 
            // clientResolutionComboBox
            // 
            this.clientResolutionComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.clientResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clientResolutionComboBox.FormattingEnabled = true;
            this.clientResolutionComboBox.Items.AddRange(new object[] {
            "Default (800 x 600)",
            "800 x 600",
            "1024 x 768",
            "1280 x 1024"});
            this.clientResolutionComboBox.Location = new System.Drawing.Point(3, 15);
            this.clientResolutionComboBox.Name = "clientResolutionComboBox";
            this.clientResolutionComboBox.Size = new System.Drawing.Size(264, 21);
            this.clientResolutionComboBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.colorDepthComboBox);
            this.groupBox1.Location = new System.Drawing.Point(16, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 38);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color Depth";
            // 
            // colorDepthComboBox
            // 
            this.colorDepthComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.colorDepthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorDepthComboBox.FormattingEnabled = true;
            this.colorDepthComboBox.Items.AddRange(new object[] {
            "Min Color (16 bit)",
            "Max Color (32 bit)"});
            this.colorDepthComboBox.Location = new System.Drawing.Point(3, 14);
            this.colorDepthComboBox.Name = "colorDepthComboBox";
            this.colorDepthComboBox.Size = new System.Drawing.Size(265, 21);
            this.colorDepthComboBox.TabIndex = 2;
            // 
            // ClientSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(299, 313);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.windowModeCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ClientSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Client Settings";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeTrackBar)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox windowModeCheckBox;
        private System.Windows.Forms.CheckBox musicActiveCheckBox;
        private System.Windows.Forms.CheckBox soundActiveCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox clientLanguageComboBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TrackBar soundVolumeTrackBar;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox clientResolutionComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox colorDepthComboBox;
    }
}

