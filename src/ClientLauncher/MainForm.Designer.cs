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
            _launchButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            MainExePathTextBox = new System.Windows.Forms.TextBox();
            SearchMainExeButton = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            configurationDialogButton = new System.Windows.Forms.Button();
            _editHostButton = new System.Windows.Forms.Button();
            _addHostButton = new System.Windows.Forms.Button();
            _serversComboBox = new System.Windows.Forms.ComboBox();
            _removeHostButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // _launchButton
            // 
            _launchButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            _launchButton.Location = new System.Drawing.Point(505, 45);
            _launchButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            _launchButton.Name = "_launchButton";
            _launchButton.Size = new System.Drawing.Size(147, 27);
            _launchButton.TabIndex = 0;
            _launchButton.Text = "Launch Client";
            _launchButton.UseVisualStyleBackColor = true;
            _launchButton.Click += LaunchClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 17);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(85, 15);
            label1.TabIndex = 1;
            label1.Text = "main.exe Path:";
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "main.exe";
            openFileDialog.Filter = "Executeables|*.exe";
            // 
            // MainExePathTextBox
            // 
            MainExePathTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MainExePathTextBox.Location = new System.Drawing.Point(107, 14);
            MainExePathTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MainExePathTextBox.Name = "MainExePathTextBox";
            MainExePathTextBox.Size = new System.Drawing.Size(467, 23);
            MainExePathTextBox.TabIndex = 2;
            MainExePathTextBox.Text = "main.exe";
            // 
            // SearchMainExeButton
            // 
            SearchMainExeButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            SearchMainExeButton.Location = new System.Drawing.Point(582, 12);
            SearchMainExeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchMainExeButton.Name = "SearchMainExeButton";
            SearchMainExeButton.Size = new System.Drawing.Size(35, 27);
            SearchMainExeButton.TabIndex = 3;
            SearchMainExeButton.Text = "...";
            SearchMainExeButton.UseVisualStyleBackColor = true;
            SearchMainExeButton.Click += SearchMainExeButtonClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(57, 54);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(42, 15);
            label2.TabIndex = 4;
            label2.Text = "Server:";
            // 
            // configurationDialogButton
            // 
            configurationDialogButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            configurationDialogButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            configurationDialogButton.Image = Properties.Resources.Settings_16x;
            configurationDialogButton.Location = new System.Drawing.Point(625, 12);
            configurationDialogButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            configurationDialogButton.Name = "configurationDialogButton";
            configurationDialogButton.Size = new System.Drawing.Size(27, 27);
            configurationDialogButton.TabIndex = 4;
            configurationDialogButton.UseVisualStyleBackColor = true;
            configurationDialogButton.Click += ConfigurationDialogButtonClick;
            // 
            // _editHostButton
            // 
            _editHostButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            _editHostButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _editHostButton.Image = Properties.Resources.Edit_16x;
            _editHostButton.Location = new System.Drawing.Point(446, 47);
            _editHostButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            _editHostButton.Name = "_editHostButton";
            _editHostButton.Size = new System.Drawing.Size(23, 23);
            _editHostButton.TabIndex = 7;
            _editHostButton.UseVisualStyleBackColor = true;
            _editHostButton.Click += OnEditHostButtonClick;
            // 
            // _addHostButton
            // 
            _addHostButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            _addHostButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _addHostButton.Image = Properties.Resources.Add_16x;
            _addHostButton.Location = new System.Drawing.Point(419, 47);
            _addHostButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            _addHostButton.Name = "_addHostButton";
            _addHostButton.Size = new System.Drawing.Size(23, 23);
            _addHostButton.TabIndex = 6;
            _addHostButton.UseVisualStyleBackColor = true;
            _addHostButton.Click += OnAddHostButtonClick;
            // 
            // _serversComboBox
            // 
            _serversComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _serversComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _serversComboBox.FormattingEnabled = true;
            _serversComboBox.Location = new System.Drawing.Point(107, 48);
            _serversComboBox.Name = "_serversComboBox";
            _serversComboBox.Size = new System.Drawing.Size(307, 23);
            _serversComboBox.TabIndex = 5;
            _serversComboBox.SelectedValueChanged += OnServersComboBoxSelectedIndexChanged;
            // 
            // _removeHostButton
            // 
            _removeHostButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            _removeHostButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _removeHostButton.Image = Properties.Resources.Remove_16x;
            _removeHostButton.Location = new System.Drawing.Point(473, 46);
            _removeHostButton.Margin = new System.Windows.Forms.Padding(2, 3, 5, 3);
            _removeHostButton.Name = "_removeHostButton";
            _removeHostButton.Size = new System.Drawing.Size(23, 23);
            _removeHostButton.TabIndex = 8;
            _removeHostButton.UseVisualStyleBackColor = true;
            _removeHostButton.Click += OnRemoveHostButtonClick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(665, 82);
            Controls.Add(_removeHostButton);
            Controls.Add(_serversComboBox);
            Controls.Add(_addHostButton);
            Controls.Add(_editHostButton);
            Controls.Add(configurationDialogButton);
            Controls.Add(label2);
            Controls.Add(SearchMainExeButton);
            Controls.Add(MainExePathTextBox);
            Controls.Add(label1);
            Controls.Add(_launchButton);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1257, 121);
            Name = "MainForm";
            Text = "MU Game Client Launcher";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button _launchButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox MainExePathTextBox;
        private System.Windows.Forms.Button SearchMainExeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button configurationDialogButton;
        private System.Windows.Forms.Button _editHostButton;
        private System.Windows.Forms.Button _addHostButton;
        private System.Windows.Forms.ComboBox _serversComboBox;
        private System.Windows.Forms.Button _removeHostButton;
    }
}

