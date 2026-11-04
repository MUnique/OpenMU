namespace MUnique.OpenMU.ClientLauncher;

partial class HostConfigurationDialog
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
        _serverPortControl = new System.Windows.Forms.NumericUpDown();
        _serverAddressTextBox = new System.Windows.Forms.TextBox();
        label2 = new System.Windows.Forms.Label();
        label1 = new System.Windows.Forms.Label();
        _descriptionTextBox = new System.Windows.Forms.TextBox();
        button1 = new System.Windows.Forms.Button();
        _cancelButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)_serverPortControl).BeginInit();
        SuspendLayout();
        // 
        // _serverPortControl
        // 
        _serverPortControl.Location = new System.Drawing.Point(273, 36);
        _serverPortControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _serverPortControl.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        _serverPortControl.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _serverPortControl.Name = "_serverPortControl";
        _serverPortControl.Size = new System.Drawing.Size(68, 23);
        _serverPortControl.TabIndex = 10;
        _serverPortControl.Value = new decimal(new int[] { 44405, 0, 0, 0 });
        // 
        // _serverAddressTextBox
        // 
        _serverAddressTextBox.Location = new System.Drawing.Point(113, 35);
        _serverAddressTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _serverAddressTextBox.Name = "_serverAddressTextBox";
        _serverAddressTextBox.Size = new System.Drawing.Size(152, 23);
        _serverAddressTextBox.TabIndex = 9;
        _serverAddressTextBox.Text = "127.127.127.127";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(13, 38);
        label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(89, 15);
        label2.TabIndex = 8;
        label2.Text = "Server-Address:";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(13, 9);
        label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(70, 15);
        label1.TabIndex = 11;
        label1.Text = "Description:";
        // 
        // _descriptionTextBox
        // 
        _descriptionTextBox.Location = new System.Drawing.Point(113, 6);
        _descriptionTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _descriptionTextBox.Name = "_descriptionTextBox";
        _descriptionTextBox.PlaceholderText = "<Enter a description here>";
        _descriptionTextBox.Size = new System.Drawing.Size(228, 23);
        _descriptionTextBox.TabIndex = 12;
        // 
        // button1
        // 
        button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        button1.DialogResult = System.Windows.Forms.DialogResult.OK;
        button1.Location = new System.Drawing.Point(194, 83);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(75, 23);
        button1.TabIndex = 13;
        button1.Text = "OK";
        button1.UseVisualStyleBackColor = true;
        // 
        // _cancelButton
        // 
        _cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        _cancelButton.Location = new System.Drawing.Point(113, 83);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new System.Drawing.Size(75, 23);
        _cancelButton.TabIndex = 14;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // HostConfiguration
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(362, 118);
        Controls.Add(_cancelButton);
        Controls.Add(button1);
        Controls.Add(_descriptionTextBox);
        Controls.Add(label1);
        Controls.Add(_serverPortControl);
        Controls.Add(_serverAddressTextBox);
        Controls.Add(label2);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        MinimizeBox = false;
        Name = "HostConfiguration";
        Text = "Configure Connection";
        ((System.ComponentModel.ISupportInitialize)_serverPortControl).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.NumericUpDown _serverPortControl;
    private System.Windows.Forms.TextBox _serverAddressTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox _descriptionTextBox;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button _cancelButton;
}