namespace MUnique.OpenMU.Network.Analyzer
{
    partial class PacketSenderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.IContainer components = null!;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.connection is not null)
                {
                    this.connection.PropertyChanged -= this.ConnectionOnPropertyChanged;
                }

                this.components?.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PacketSenderForm));
            this.packetTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.toClientRadioButton = new System.Windows.Forms.RadioButton();
            this.toServerRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // packetTextBox
            // 
            this.packetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.packetTextBox.Location = new System.Drawing.Point(12, 12);
            this.packetTextBox.Multiline = true;
            this.packetTextBox.Name = "packetTextBox";
            this.packetTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.packetTextBox.Size = new System.Drawing.Size(567, 76);
            this.packetTextBox.TabIndex = 4;
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sendButton.Location = new System.Drawing.Point(504, 94);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 7;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            // 
            // toClientRadioButton
            // 
            this.toClientRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toClientRadioButton.AutoSize = true;
            this.toClientRadioButton.Checked = true;
            this.toClientRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toClientRadioButton.Location = new System.Drawing.Point(12, 98);
            this.toClientRadioButton.Name = "toClientRadioButton";
            this.toClientRadioButton.Size = new System.Drawing.Size(71, 19);
            this.toClientRadioButton.TabIndex = 6;
            this.toClientRadioButton.TabStop = true;
            this.toClientRadioButton.Text = "To Client";
            this.toClientRadioButton.UseVisualStyleBackColor = true;
            // 
            // toServerRadioButton
            // 
            this.toServerRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toServerRadioButton.AutoSize = true;
            this.toServerRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toServerRadioButton.Location = new System.Drawing.Point(84, 98);
            this.toServerRadioButton.Name = "toServerRadioButton";
            this.toServerRadioButton.Size = new System.Drawing.Size(72, 19);
            this.toServerRadioButton.TabIndex = 5;
            this.toServerRadioButton.Text = "To Server";
            this.toServerRadioButton.UseVisualStyleBackColor = true;
            // 
            // PacketSenderForm
            // 
            this.ClientSize = new System.Drawing.Size(591, 129);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.toClientRadioButton);
            this.Controls.Add(this.toServerRadioButton);
            this.Controls.Add(this.packetTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PacketSenderForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox packetTextBox = null!;
        private System.Windows.Forms.Button sendButton = null!;
        private System.Windows.Forms.RadioButton toClientRadioButton = null!;
        private System.Windows.Forms.RadioButton toServerRadioButton = null!;
    }
}
