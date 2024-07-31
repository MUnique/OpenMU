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
            windowModeCheckBox = new System.Windows.Forms.CheckBox();
            musicActiveCheckBox = new System.Windows.Forms.CheckBox();
            soundActiveCheckBox = new System.Windows.Forms.CheckBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            soundVolumeTrackBar = new System.Windows.Forms.TrackBar();
            groupBox4 = new System.Windows.Forms.GroupBox();
            clientLanguageComboBox = new System.Windows.Forms.ComboBox();
            saveButton = new System.Windows.Forms.Button();
            closeButton = new System.Windows.Forms.Button();
            groupBox5 = new System.Windows.Forms.GroupBox();
            clientResolutionComboBox = new System.Windows.Forms.ComboBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            colorDepthComboBox = new System.Windows.Forms.ComboBox();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)soundVolumeTrackBar).BeginInit();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // windowModeCheckBox
            // 
            windowModeCheckBox.AutoSize = true;
            windowModeCheckBox.Location = new System.Drawing.Point(22, 286);
            windowModeCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            windowModeCheckBox.Name = "windowModeCheckBox";
            windowModeCheckBox.Size = new System.Drawing.Size(104, 19);
            windowModeCheckBox.TabIndex = 0;
            windowModeCheckBox.Text = "Window Mode";
            windowModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // musicActiveCheckBox
            // 
            musicActiveCheckBox.AutoSize = true;
            musicActiveCheckBox.Location = new System.Drawing.Point(136, 22);
            musicActiveCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            musicActiveCheckBox.Name = "musicActiveCheckBox";
            musicActiveCheckBox.Size = new System.Drawing.Size(125, 19);
            musicActiveCheckBox.TabIndex = 3;
            musicActiveCheckBox.Text = "Background Music";
            musicActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // soundActiveCheckBox
            // 
            soundActiveCheckBox.AutoSize = true;
            soundActiveCheckBox.Location = new System.Drawing.Point(10, 22);
            soundActiveCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            soundActiveCheckBox.Name = "soundActiveCheckBox";
            soundActiveCheckBox.Size = new System.Drawing.Size(98, 19);
            soundActiveCheckBox.TabIndex = 4;
            soundActiveCheckBox.Text = "Sound Effects";
            soundActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(soundVolumeTrackBar);
            groupBox3.Controls.Add(musicActiveCheckBox);
            groupBox3.Controls.Add(soundActiveCheckBox);
            groupBox3.Location = new System.Drawing.Point(19, 115);
            groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Size = new System.Drawing.Size(315, 111);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            groupBox3.Text = "Sound";
            // 
            // soundVolumeTrackBar
            // 
            soundVolumeTrackBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            soundVolumeTrackBar.Location = new System.Drawing.Point(4, 63);
            soundVolumeTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            soundVolumeTrackBar.Name = "soundVolumeTrackBar";
            soundVolumeTrackBar.Size = new System.Drawing.Size(307, 45);
            soundVolumeTrackBar.TabIndex = 5;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(clientLanguageComboBox);
            groupBox4.Location = new System.Drawing.Point(19, 233);
            groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox4.Size = new System.Drawing.Size(315, 46);
            groupBox4.TabIndex = 8;
            groupBox4.TabStop = false;
            groupBox4.Text = "Language";
            // 
            // clientLanguageComboBox
            // 
            clientLanguageComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            clientLanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            clientLanguageComboBox.FormattingEnabled = true;
            clientLanguageComboBox.Items.AddRange(new object[] { "English", "Portuguese", "Spanish" });
            clientLanguageComboBox.Location = new System.Drawing.Point(4, 20);
            clientLanguageComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            clientLanguageComboBox.Name = "clientLanguageComboBox";
            clientLanguageComboBox.Size = new System.Drawing.Size(307, 23);
            clientLanguageComboBox.TabIndex = 0;
            // 
            // saveButton
            // 
            saveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            saveButton.Location = new System.Drawing.Point(153, 321);
            saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(88, 27);
            saveButton.TabIndex = 9;
            saveButton.Text = "OK";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButtonClick;
            // 
            // closeButton
            // 
            closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            closeButton.Location = new System.Drawing.Point(247, 321);
            closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(88, 27);
            closeButton.TabIndex = 10;
            closeButton.Text = "Cancel";
            closeButton.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(clientResolutionComboBox);
            groupBox5.Location = new System.Drawing.Point(19, 13);
            groupBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox5.Size = new System.Drawing.Size(315, 45);
            groupBox5.TabIndex = 11;
            groupBox5.TabStop = false;
            groupBox5.Text = "Screen Resolution";
            // 
            // clientResolutionComboBox
            // 
            clientResolutionComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            clientResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            clientResolutionComboBox.FormattingEnabled = true;
            clientResolutionComboBox.Items.AddRange(new object[] { "Default (800 x 600)", "800 x 600", "1024 x 768", "1280 x 1024" });
            clientResolutionComboBox.Location = new System.Drawing.Point(4, 19);
            clientResolutionComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            clientResolutionComboBox.Name = "clientResolutionComboBox";
            clientResolutionComboBox.Size = new System.Drawing.Size(307, 23);
            clientResolutionComboBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(colorDepthComboBox);
            groupBox1.Location = new System.Drawing.Point(19, 65);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(316, 44);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Color Depth";
            // 
            // colorDepthComboBox
            // 
            colorDepthComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            colorDepthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            colorDepthComboBox.FormattingEnabled = true;
            colorDepthComboBox.Items.AddRange(new object[] { "Min Color (16 bit)", "Max Color (32 bit)" });
            colorDepthComboBox.Location = new System.Drawing.Point(4, 18);
            colorDepthComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            colorDepthComboBox.Name = "colorDepthComboBox";
            colorDepthComboBox.Size = new System.Drawing.Size(308, 23);
            colorDepthComboBox.TabIndex = 2;
            // 
            // ClientSettingsDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = closeButton;
            ClientSize = new System.Drawing.Size(349, 361);
            Controls.Add(groupBox1);
            Controls.Add(groupBox5);
            Controls.Add(closeButton);
            Controls.Add(saveButton);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(windowModeCheckBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ClientSettingsDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Change Client Settings";
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)soundVolumeTrackBar).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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

