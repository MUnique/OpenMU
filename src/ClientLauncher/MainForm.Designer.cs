namespace MUnique.OpenMU.ClientLauncher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LaunchButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MainExePathTextBox = new System.Windows.Forms.TextBox();
            this.SearchMainExeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ServerAddressTextBox = new System.Windows.Forms.TextBox();
            this.ServerPortControl = new System.Windows.Forms.NumericUpDown();
            this.configurationDialogButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ServerPortControl)).BeginInit();
            this.SuspendLayout();
            // 
            // LaunchButton
            // 
            this.LaunchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LaunchButton.Location = new System.Drawing.Point(455, 42);
            this.LaunchButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LaunchButton.Name = "LaunchButton";
            this.LaunchButton.Size = new System.Drawing.Size(147, 27);
            this.LaunchButton.TabIndex = 0;
            this.LaunchButton.Text = "Launch Client";
            this.LaunchButton.UseVisualStyleBackColor = true;
            this.LaunchButton.Click += new System.EventHandler(this.LaunchClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "main.exe Path:";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "main.exe";
            this.openFileDialog.Filter = "Executeables|*.exe";
            // 
            // MainExePathTextBox
            // 
            this.MainExePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainExePathTextBox.Location = new System.Drawing.Point(113, 14);
            this.MainExePathTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MainExePathTextBox.Name = "MainExePathTextBox";
            this.MainExePathTextBox.Size = new System.Drawing.Size(446, 23);
            this.MainExePathTextBox.TabIndex = 2;
            this.MainExePathTextBox.Text = "main.exe";
            // 
            // SearchMainExeButton
            // 
            this.SearchMainExeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchMainExeButton.Location = new System.Drawing.Point(567, 12);
            this.SearchMainExeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SearchMainExeButton.Name = "SearchMainExeButton";
            this.SearchMainExeButton.Size = new System.Drawing.Size(35, 27);
            this.SearchMainExeButton.TabIndex = 3;
            this.SearchMainExeButton.Text = "...";
            this.SearchMainExeButton.UseVisualStyleBackColor = true;
            this.SearchMainExeButton.Click += new System.EventHandler(this.SearchMainExeButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Server-Address:";
            // 
            // ServerAddressTextBox
            // 
            this.ServerAddressTextBox.Location = new System.Drawing.Point(113, 44);
            this.ServerAddressTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ServerAddressTextBox.Name = "ServerAddressTextBox";
            this.ServerAddressTextBox.Size = new System.Drawing.Size(152, 23);
            this.ServerAddressTextBox.TabIndex = 5;
            this.ServerAddressTextBox.Text = "127.127.127.127";
            // 
            // ServerPortControl
            // 
            this.ServerPortControl.Location = new System.Drawing.Point(273, 44);
            this.ServerPortControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ServerPortControl.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ServerPortControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ServerPortControl.Name = "ServerPortControl";
            this.ServerPortControl.Size = new System.Drawing.Size(68, 23);
            this.ServerPortControl.TabIndex = 7;
            this.ServerPortControl.Value = new decimal(new int[] {
            44405,
            0,
            0,
            0});
            // 
            // configurationDialogButton
            // 
            this.configurationDialogButton.AutoSize = true;
            this.configurationDialogButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configurationDialogButton.Image = global::MUnique.OpenMU.ClientLauncher.Properties.Resources.Settings_16x;
            this.configurationDialogButton.Location = new System.Drawing.Point(348, 43);
            this.configurationDialogButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.configurationDialogButton.Name = "configurationDialogButton";
            this.configurationDialogButton.Size = new System.Drawing.Size(22, 22);
            this.configurationDialogButton.TabIndex = 8;
            this.configurationDialogButton.UseVisualStyleBackColor = true;
            this.configurationDialogButton.Click += new System.EventHandler(this.ConfigurationDialogButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 82);
            this.Controls.Add(this.configurationDialogButton);
            this.Controls.Add(this.ServerPortControl);
            this.Controls.Add(this.ServerAddressTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SearchMainExeButton);
            this.Controls.Add(this.MainExePathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LaunchButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1257, 121);
            this.Name = "MainForm";
            this.Text = "Game Client Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.ServerPortControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LaunchButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox MainExePathTextBox;
        private System.Windows.Forms.Button SearchMainExeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ServerAddressTextBox;
        private System.Windows.Forms.NumericUpDown ServerPortControl;
        private System.Windows.Forms.Button configurationDialogButton;
    }
}

