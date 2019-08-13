namespace MUnique.OpenMU.Network.Analyzer
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.packetGridView = new System.Windows.Forms.DataGridView();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PacketSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PacketData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.packetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.rawDataTextBox = new System.Windows.Forms.TextBox();
            this.packetInfoGroup = new System.Windows.Forms.GroupBox();
            this.extractedInfoTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.settingsGroup = new System.Windows.Forms.GroupBox();
            this.targetPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.targetHostTextBox = new System.Windows.Forms.TextBox();
            this.listenerPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.btnStartProxy = new System.Windows.Forms.Button();
            this.connectedClientsGroup = new System.Windows.Forms.GroupBox();
            this.connectedClientsListBox = new System.Windows.Forms.ListBox();
            this.connectionContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPacketSenderStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientVersionComboBox = new System.Windows.Forms.ComboBox();
            this.clientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.trafficGroup = new System.Windows.Forms.GroupBox();
            this.leftPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.packetGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.packetBindingSource)).BeginInit();
            this.packetInfoGroup.SuspendLayout();
            this.settingsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetPortNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listenerPortNumericUpDown)).BeginInit();
            this.connectedClientsGroup.SuspendLayout();
            this.connectionContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientBindingSource)).BeginInit();
            this.trafficGroup.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // packetGridView
            // 
            this.packetGridView.AllowUserToAddRows = false;
            this.packetGridView.AllowUserToOrderColumns = true;
            this.packetGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetGridView.AutoGenerateColumns = false;
            this.packetGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.packetGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timestamp,
            this.PacketSize,
            this.Type,
            this.Code,
            this.Direction,
            this.PacketData});
            this.packetGridView.DataSource = this.packetBindingSource;
            this.packetGridView.Location = new System.Drawing.Point(3, 16);
            this.packetGridView.MultiSelect = false;
            this.packetGridView.Name = "packetGridView";
            this.packetGridView.ReadOnly = true;
            this.packetGridView.RowHeadersWidth = 20;
            this.packetGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.packetGridView.Size = new System.Drawing.Size(509, 597);
            this.packetGridView.TabIndex = 27;
            this.packetGridView.SelectionChanged += new System.EventHandler(this.PacketSelected);
            // 
            // Timestamp
            // 
            this.Timestamp.DataPropertyName = "Timestamp";
            dataGridViewCellStyle1.Format = "o";
            this.Timestamp.DefaultCellStyle = dataGridViewCellStyle1;
            this.Timestamp.HeaderText = "Timestamp";
            this.Timestamp.Name = "Timestamp";
            this.Timestamp.ReadOnly = true;
            this.Timestamp.Width = 125;
            // 
            // PacketSize
            // 
            this.PacketSize.DataPropertyName = "Size";
            this.PacketSize.HeaderText = "Size";
            this.PacketSize.Name = "PacketSize";
            this.PacketSize.ReadOnly = true;
            this.PacketSize.Width = 50;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            dataGridViewCellStyle2.Format = "X2";
            this.Type.DefaultCellStyle = dataGridViewCellStyle2;
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 50;
            // 
            // Code
            // 
            this.Code.DataPropertyName = "Code";
            dataGridViewCellStyle3.Format = "X2";
            this.Code.DefaultCellStyle = dataGridViewCellStyle3;
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.Width = 50;
            // 
            // Direction
            // 
            this.Direction.DataPropertyName = "Direction";
            this.Direction.HeaderText = "Direction";
            this.Direction.Name = "Direction";
            this.Direction.ReadOnly = true;
            this.Direction.Width = 75;
            // 
            // PacketData
            // 
            this.PacketData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PacketData.DataPropertyName = "PacketData";
            this.PacketData.HeaderText = "Data";
            this.PacketData.Name = "PacketData";
            this.PacketData.ReadOnly = true;
            // 
            // packetBindingSource
            // 
            this.packetBindingSource.AllowNew = false;
            // 
            // rawDataTextBox
            // 
            this.rawDataTextBox.Location = new System.Drawing.Point(6, 277);
            this.rawDataTextBox.Multiline = true;
            this.rawDataTextBox.Name = "rawDataTextBox";
            this.rawDataTextBox.ReadOnly = true;
            this.rawDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rawDataTextBox.Size = new System.Drawing.Size(354, 216);
            this.rawDataTextBox.TabIndex = 29;
            // 
            // packetInfoGroup
            // 
            this.packetInfoGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetInfoGroup.Controls.Add(this.extractedInfoTextBox);
            this.packetInfoGroup.Controls.Add(this.label3);
            this.packetInfoGroup.Controls.Add(this.label1);
            this.packetInfoGroup.Controls.Add(this.rawDataTextBox);
            this.packetInfoGroup.Location = new System.Drawing.Point(518, 16);
            this.packetInfoGroup.Name = "packetInfoGroup";
            this.packetInfoGroup.Size = new System.Drawing.Size(366, 597);
            this.packetInfoGroup.TabIndex = 30;
            this.packetInfoGroup.TabStop = false;
            this.packetInfoGroup.Text = "Packet Info";
            // 
            // extractedInfoTextBox
            // 
            this.extractedInfoTextBox.Location = new System.Drawing.Point(6, 32);
            this.extractedInfoTextBox.Multiline = true;
            this.extractedInfoTextBox.Name = "extractedInfoTextBox";
            this.extractedInfoTextBox.ReadOnly = true;
            this.extractedInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.extractedInfoTextBox.Size = new System.Drawing.Size(354, 200);
            this.extractedInfoTextBox.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Extracted Information:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Raw Data:";
            // 
            // settingsGroup
            // 
            this.settingsGroup.Controls.Add(this.targetPortNumericUpDown);
            this.settingsGroup.Controls.Add(this.label6);
            this.settingsGroup.Controls.Add(this.label7);
            this.settingsGroup.Controls.Add(this.label4);
            this.settingsGroup.Controls.Add(this.label8);
            this.settingsGroup.Controls.Add(this.targetHostTextBox);
            this.settingsGroup.Controls.Add(this.listenerPortNumericUpDown);
            this.settingsGroup.Controls.Add(this.btnStartProxy);
            this.settingsGroup.Controls.Add(this.clientVersionComboBox);
            this.settingsGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.settingsGroup.Location = new System.Drawing.Point(0, 0);
            this.settingsGroup.Name = "settingsGroup";
            this.settingsGroup.Size = new System.Drawing.Size(200, 160);
            this.settingsGroup.TabIndex = 32;
            this.settingsGroup.TabStop = false;
            this.settingsGroup.Text = "Settings";
            // 
            // targetPortNumericUpDown
            // 
            this.targetPortNumericUpDown.Location = new System.Drawing.Point(73, 45);
            this.targetPortNumericUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.targetPortNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.targetPortNumericUpDown.Name = "targetPortNumericUpDown";
            this.targetPortNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.targetPortNumericUpDown.TabIndex = 3;
            this.targetPortNumericUpDown.Value = new decimal(new int[] {
            55901,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Target IP:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(6, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Target Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Local Port:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(6, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "MU Version:";
            // 
            // targetHostTextBox
            // 
            this.targetHostTextBox.Location = new System.Drawing.Point(73, 19);
            this.targetHostTextBox.MaxLength = 15;
            this.targetHostTextBox.Name = "targetHostTextBox";
            this.targetHostTextBox.Size = new System.Drawing.Size(95, 20);
            this.targetHostTextBox.TabIndex = 1;
            this.targetHostTextBox.Text = "127.127.127.127";
            // 
            // listenerPortNumericUpDown
            // 
            this.listenerPortNumericUpDown.Location = new System.Drawing.Point(73, 71);
            this.listenerPortNumericUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.listenerPortNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.listenerPortNumericUpDown.Name = "listenerPortNumericUpDown";
            this.listenerPortNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.listenerPortNumericUpDown.TabIndex = 3;
            this.listenerPortNumericUpDown.Value = new decimal(new int[] {
            55900,
            0,
            0,
            0});
            // 
            // clientVersionComboBox
            //
            this.clientVersionComboBox.Location = new System.Drawing.Point(73, 97);
            this.clientVersionComboBox.Name = "clientVersionComboBox";
            this.clientVersionComboBox.Size = new System.Drawing.Size(100, 20);
            this.clientVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clientVersionComboBox.TabIndex = 4;
            // 
            // btnStartProxy
            // 
            this.btnStartProxy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStartProxy.Location = new System.Drawing.Point(73, 123);
            this.btnStartProxy.Name = "btnStartProxy";
            this.btnStartProxy.Size = new System.Drawing.Size(79, 23);
            this.btnStartProxy.TabIndex = 5;
            this.btnStartProxy.Text = "Start Proxy";
            this.btnStartProxy.UseVisualStyleBackColor = true;
            this.btnStartProxy.Click += new System.EventHandler(this.StartProxy);
            // 
            // connectedClientsGroup
            // 
            this.connectedClientsGroup.Controls.Add(this.connectedClientsListBox);
            this.connectedClientsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectedClientsGroup.Location = new System.Drawing.Point(0, 135);
            this.connectedClientsGroup.Name = "connectedClientsGroup";
            this.connectedClientsGroup.Size = new System.Drawing.Size(200, 484);
            this.connectedClientsGroup.TabIndex = 33;
            this.connectedClientsGroup.TabStop = false;
            this.connectedClientsGroup.Text = "Connected Clients";
            // 
            // connectedClientsListBox
            // 
            this.connectedClientsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.connectedClientsListBox.ContextMenuStrip = this.connectionContextMenu;
            this.connectedClientsListBox.DataSource = this.clientBindingSource;
            this.connectedClientsListBox.FormattingEnabled = true;
            this.connectedClientsListBox.Location = new System.Drawing.Point(3, 16);
            this.connectedClientsListBox.Name = "connectedClientsListBox";
            this.connectedClientsListBox.Size = new System.Drawing.Size(197, 459);
            this.connectedClientsListBox.TabIndex = 0;
            this.connectedClientsListBox.SelectedIndexChanged += new System.EventHandler(this.ConnectionSelected);
            // 
            // connectionContextMenu
            // 
            this.connectionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disconnectToolStripMenuItem, this.loadToolStripMenuItem, this.saveToolStripMenuItem, this.openPacketSenderStripMenuItem});
            this.connectionContextMenu.Name = "connectionContextMenu";
            this.connectionContextMenu.Size = new System.Drawing.Size(153, 48);
            this.connectionContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.BeforeContextMenuOpens);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.DisconnectClient);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Load from file";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadFromFile);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save to file";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToFile);
            // 
            // openPacketSenderStripMenuItem
            // 
            this.openPacketSenderStripMenuItem.Name = "openPacketSenderStripMenuItem";
            this.openPacketSenderStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openPacketSenderStripMenuItem.Text = "Packet Sender";
            this.openPacketSenderStripMenuItem.Click += new System.EventHandler(this.SendPacket);
            // 
            // trafficGroup
            // 
            this.trafficGroup.Controls.Add(this.packetInfoGroup);
            this.trafficGroup.Controls.Add(this.packetGridView);
            this.trafficGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trafficGroup.Enabled = false;
            this.trafficGroup.Location = new System.Drawing.Point(200, 0);
            this.trafficGroup.Name = "trafficGroup";
            this.trafficGroup.Size = new System.Drawing.Size(890, 619);
            this.trafficGroup.TabIndex = 34;
            this.trafficGroup.TabStop = false;
            this.trafficGroup.Text = "Traffic";
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.connectedClientsGroup);
            this.leftPanel.Controls.Add(this.settingsGroup);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(200, 619);
            this.leftPanel.TabIndex = 35;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1090, 619);
            this.Controls.Add(this.trafficGroup);
            this.Controls.Add(this.leftPanel);
            this.Name = "MainForm";
            this.Text = "MUnique OpenMU Network Analyzer";
            ((System.ComponentModel.ISupportInitialize)(this.packetGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.packetBindingSource)).EndInit();
            this.packetInfoGroup.ResumeLayout(false);
            this.packetInfoGroup.PerformLayout();
            this.settingsGroup.ResumeLayout(false);
            this.settingsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetPortNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listenerPortNumericUpDown)).EndInit();
            this.connectedClientsGroup.ResumeLayout(false);
            this.connectionContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.clientBindingSource)).EndInit();
            this.trafficGroup.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView packetGridView;
        private System.Windows.Forms.TextBox rawDataTextBox;
        private System.Windows.Forms.GroupBox packetInfoGroup;
        private System.Windows.Forms.TextBox extractedInfoTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox settingsGroup;
        private System.Windows.Forms.TextBox targetHostTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown targetPortNumericUpDown;
        private System.Windows.Forms.NumericUpDown listenerPortNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnStartProxy;
        private System.Windows.Forms.GroupBox connectedClientsGroup;
        private System.Windows.Forms.ListBox connectedClientsListBox;
        private System.Windows.Forms.GroupBox trafficGroup;
        private System.Windows.Forms.BindingSource clientBindingSource;
        private System.Windows.Forms.BindingSource packetBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn PacketSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn PacketData;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.ContextMenuStrip connectionContextMenu;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPacketSenderStripMenuItem;
        private System.Windows.Forms.ComboBox clientVersionComboBox;
    }
}

