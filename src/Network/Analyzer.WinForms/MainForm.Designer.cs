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
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            packetGridView = new Zuby.ADGV.AdvancedDataGridView();
            packetBindingSource = new System.Windows.Forms.BindingSource(components);
            rawDataTextBox = new System.Windows.Forms.TextBox();
            packetInfoGroup = new System.Windows.Forms.GroupBox();
            extractedInfoTextBox = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            settingsGroup = new System.Windows.Forms.GroupBox();
            targetPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            targetHostTextBox = new System.Windows.Forms.TextBox();
            listenerPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            btnStartProxy = new System.Windows.Forms.Button();
            clientVersionComboBox = new System.Windows.Forms.ComboBox();
            connectedClientsGroup = new System.Windows.Forms.GroupBox();
            connectedClientsListBox = new System.Windows.Forms.ListBox();
            connectionContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openPacketSenderStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            clientBindingSource = new System.Windows.Forms.BindingSource(components);
            trafficGroup = new System.Windows.Forms.GroupBox();
            leftPanel = new System.Windows.Forms.Panel();
            Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            PacketSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            PacketData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)packetGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)packetBindingSource).BeginInit();
            packetInfoGroup.SuspendLayout();
            settingsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)targetPortNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)listenerPortNumericUpDown).BeginInit();
            connectedClientsGroup.SuspendLayout();
            connectionContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)clientBindingSource).BeginInit();
            trafficGroup.SuspendLayout();
            leftPanel.SuspendLayout();
            SuspendLayout();
            // 
            // packetGridView
            // 
            packetGridView.AllowUserToAddRows = false;
            packetGridView.AllowUserToOrderColumns = true;
            packetGridView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            packetGridView.AutoGenerateColumns = false;
            packetGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            packetGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Timestamp, PacketSize, Type, Code, Direction, Message, PacketData });
            packetGridView.DataSource = packetBindingSource;
            packetGridView.FilterAndSortEnabled = true;
            packetGridView.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            packetGridView.Location = new System.Drawing.Point(4, 18);
            packetGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            packetGridView.MaxFilterButtonImageHeight = 23;
            packetGridView.MultiSelect = false;
            packetGridView.Name = "packetGridView";
            packetGridView.ReadOnly = true;
            packetGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            packetGridView.RowHeadersWidth = 20;
            packetGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            packetGridView.Size = new System.Drawing.Size(595, 689);
            packetGridView.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            packetGridView.TabIndex = 27;
            packetGridView.SelectionChanged += OnPacketSelected;
            // 
            // packetBindingSource
            // 
            packetBindingSource.AllowNew = false;
            // 
            // rawDataTextBox
            // 
            rawDataTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            rawDataTextBox.Location = new System.Drawing.Point(7, 320);
            rawDataTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rawDataTextBox.Multiline = true;
            rawDataTextBox.Name = "rawDataTextBox";
            rawDataTextBox.ReadOnly = true;
            rawDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            rawDataTextBox.Size = new System.Drawing.Size(412, 363);
            rawDataTextBox.TabIndex = 29;
            // 
            // packetInfoGroup
            // 
            packetInfoGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            packetInfoGroup.Controls.Add(extractedInfoTextBox);
            packetInfoGroup.Controls.Add(label3);
            packetInfoGroup.Controls.Add(label1);
            packetInfoGroup.Controls.Add(rawDataTextBox);
            packetInfoGroup.Location = new System.Drawing.Point(605, 18);
            packetInfoGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            packetInfoGroup.Name = "packetInfoGroup";
            packetInfoGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            packetInfoGroup.Size = new System.Drawing.Size(427, 689);
            packetInfoGroup.TabIndex = 30;
            packetInfoGroup.TabStop = false;
            packetInfoGroup.Text = "Packet Info";
            // 
            // extractedInfoTextBox
            // 
            extractedInfoTextBox.Location = new System.Drawing.Point(7, 37);
            extractedInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            extractedInfoTextBox.Multiline = true;
            extractedInfoTextBox.Name = "extractedInfoTextBox";
            extractedInfoTextBox.ReadOnly = true;
            extractedInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            extractedInfoTextBox.Size = new System.Drawing.Size(412, 230);
            extractedInfoTextBox.TabIndex = 33;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(7, 18);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(125, 15);
            label3.TabIndex = 32;
            label3.Text = "Extracted Information:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 301);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(59, 15);
            label1.TabIndex = 30;
            label1.Text = "Raw Data:";
            // 
            // settingsGroup
            // 
            settingsGroup.Controls.Add(targetPortNumericUpDown);
            settingsGroup.Controls.Add(label6);
            settingsGroup.Controls.Add(label7);
            settingsGroup.Controls.Add(label4);
            settingsGroup.Controls.Add(label8);
            settingsGroup.Controls.Add(targetHostTextBox);
            settingsGroup.Controls.Add(listenerPortNumericUpDown);
            settingsGroup.Controls.Add(btnStartProxy);
            settingsGroup.Controls.Add(clientVersionComboBox);
            settingsGroup.Dock = System.Windows.Forms.DockStyle.Top;
            settingsGroup.Location = new System.Drawing.Point(0, 0);
            settingsGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            settingsGroup.Name = "settingsGroup";
            settingsGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            settingsGroup.Size = new System.Drawing.Size(233, 185);
            settingsGroup.TabIndex = 32;
            settingsGroup.TabStop = false;
            settingsGroup.Text = "Settings";
            // 
            // targetPortNumericUpDown
            // 
            targetPortNumericUpDown.Location = new System.Drawing.Point(85, 52);
            targetPortNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            targetPortNumericUpDown.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            targetPortNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            targetPortNumericUpDown.Name = "targetPortNumericUpDown";
            targetPortNumericUpDown.Size = new System.Drawing.Size(92, 23);
            targetPortNumericUpDown.TabIndex = 3;
            targetPortNumericUpDown.Value = new decimal(new int[] { 55901, 0, 0, 0 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.SystemColors.ControlText;
            label6.Location = new System.Drawing.Point(7, 25);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(55, 15);
            label6.TabIndex = 0;
            label6.Text = "Target IP:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = System.Drawing.SystemColors.ControlText;
            label7.Location = new System.Drawing.Point(7, 54);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(67, 15);
            label7.TabIndex = 2;
            label7.Text = "Target Port:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.SystemColors.ControlText;
            label4.Location = new System.Drawing.Point(7, 84);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(63, 15);
            label4.TabIndex = 2;
            label4.Text = "Local Port:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.SystemColors.ControlText;
            label8.Location = new System.Drawing.Point(7, 114);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(70, 15);
            label8.TabIndex = 2;
            label8.Text = "MU Version:";
            // 
            // targetHostTextBox
            // 
            targetHostTextBox.Location = new System.Drawing.Point(85, 22);
            targetHostTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            targetHostTextBox.MaxLength = 15;
            targetHostTextBox.Name = "targetHostTextBox";
            targetHostTextBox.Size = new System.Drawing.Size(140, 23);
            targetHostTextBox.TabIndex = 1;
            targetHostTextBox.Text = "127.127.127.127";
            // 
            // listenerPortNumericUpDown
            // 
            listenerPortNumericUpDown.Location = new System.Drawing.Point(85, 82);
            listenerPortNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listenerPortNumericUpDown.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            listenerPortNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            listenerPortNumericUpDown.Name = "listenerPortNumericUpDown";
            listenerPortNumericUpDown.Size = new System.Drawing.Size(92, 23);
            listenerPortNumericUpDown.TabIndex = 3;
            listenerPortNumericUpDown.Value = new decimal(new int[] { 55900, 0, 0, 0 });
            // 
            // btnStartProxy
            // 
            btnStartProxy.ForeColor = System.Drawing.SystemColors.ControlText;
            btnStartProxy.Location = new System.Drawing.Point(85, 142);
            btnStartProxy.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartProxy.Name = "btnStartProxy";
            btnStartProxy.Size = new System.Drawing.Size(92, 27);
            btnStartProxy.TabIndex = 5;
            btnStartProxy.Text = "Start Proxy";
            btnStartProxy.UseVisualStyleBackColor = true;
            btnStartProxy.Click += StartProxy;
            // 
            // clientVersionComboBox
            // 
            clientVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            clientVersionComboBox.Location = new System.Drawing.Point(85, 112);
            clientVersionComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            clientVersionComboBox.Name = "clientVersionComboBox";
            clientVersionComboBox.Size = new System.Drawing.Size(140, 23);
            clientVersionComboBox.TabIndex = 4;
            // 
            // connectedClientsGroup
            // 
            connectedClientsGroup.Controls.Add(connectedClientsListBox);
            connectedClientsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            connectedClientsGroup.Location = new System.Drawing.Point(0, 185);
            connectedClientsGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            connectedClientsGroup.Name = "connectedClientsGroup";
            connectedClientsGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            connectedClientsGroup.Size = new System.Drawing.Size(233, 529);
            connectedClientsGroup.TabIndex = 33;
            connectedClientsGroup.TabStop = false;
            connectedClientsGroup.Text = "Connected Clients";
            // 
            // connectedClientsListBox
            // 
            connectedClientsListBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            connectedClientsListBox.ContextMenuStrip = connectionContextMenu;
            connectedClientsListBox.DataSource = clientBindingSource;
            connectedClientsListBox.FormattingEnabled = true;
            connectedClientsListBox.ItemHeight = 15;
            connectedClientsListBox.Location = new System.Drawing.Point(4, 18);
            connectedClientsListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            connectedClientsListBox.Name = "connectedClientsListBox";
            connectedClientsListBox.Size = new System.Drawing.Size(229, 499);
            connectedClientsListBox.TabIndex = 0;
            connectedClientsListBox.SelectedIndexChanged += OnConnectionSelected;
            // 
            // connectionContextMenu
            // 
            connectionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { disconnectToolStripMenuItem, loadToolStripMenuItem, saveToolStripMenuItem, openPacketSenderStripMenuItem });
            connectionContextMenu.Name = "connectionContextMenu";
            connectionContextMenu.Size = new System.Drawing.Size(149, 92);
            connectionContextMenu.Opening += OnBeforeContextMenuOpens;
            // 
            // disconnectToolStripMenuItem
            // 
            disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            disconnectToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            disconnectToolStripMenuItem.Text = "Disconnect";
            disconnectToolStripMenuItem.Click += OnDisconnectClientClick;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            loadToolStripMenuItem.Text = "Load from file";
            loadToolStripMenuItem.Click += OnLoadFromFileClick;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            saveToolStripMenuItem.Text = "Save to file";
            saveToolStripMenuItem.Click += OnSaveToFileClick;
            // 
            // openPacketSenderStripMenuItem
            // 
            openPacketSenderStripMenuItem.Name = "openPacketSenderStripMenuItem";
            openPacketSenderStripMenuItem.Size = new System.Drawing.Size(148, 22);
            openPacketSenderStripMenuItem.Text = "Packet Sender";
            openPacketSenderStripMenuItem.Click += OnSendPacketClick;
            // 
            // trafficGroup
            // 
            trafficGroup.Controls.Add(packetInfoGroup);
            trafficGroup.Controls.Add(packetGridView);
            trafficGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            trafficGroup.Enabled = false;
            trafficGroup.Location = new System.Drawing.Point(233, 0);
            trafficGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trafficGroup.Name = "trafficGroup";
            trafficGroup.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trafficGroup.Size = new System.Drawing.Size(1039, 714);
            trafficGroup.TabIndex = 34;
            trafficGroup.TabStop = false;
            trafficGroup.Text = "Traffic";
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(connectedClientsGroup);
            leftPanel.Controls.Add(settingsGroup);
            leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            leftPanel.Location = new System.Drawing.Point(0, 0);
            leftPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new System.Drawing.Size(233, 714);
            leftPanel.TabIndex = 35;
            // 
            // Timestamp
            // 
            Timestamp.DataPropertyName = "Timestamp";
            dataGridViewCellStyle1.Format = "g";
            Timestamp.DefaultCellStyle = dataGridViewCellStyle1;
            Timestamp.HeaderText = "Timestamp";
            Timestamp.MinimumWidth = 24;
            Timestamp.Name = "Timestamp";
            Timestamp.ReadOnly = true;
            Timestamp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // PacketSize
            // 
            PacketSize.DataPropertyName = "Size";
            PacketSize.HeaderText = "Size";
            PacketSize.MinimumWidth = 24;
            PacketSize.Name = "PacketSize";
            PacketSize.ReadOnly = true;
            PacketSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            PacketSize.Width = 55;
            // 
            // Type
            // 
            Type.DataPropertyName = "Type";
            dataGridViewCellStyle2.Format = "X2";
            Type.DefaultCellStyle = dataGridViewCellStyle2;
            Type.HeaderText = "Type";
            Type.MinimumWidth = 24;
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            Type.Width = 55;
            // 
            // Code
            // 
            Code.DataPropertyName = "DisplayCode";
            dataGridViewCellStyle3.Format = "X2";
            dataGridViewCellStyle3.NullValue = null;
            Code.DefaultCellStyle = dataGridViewCellStyle3;
            Code.HeaderText = "Code";
            Code.MinimumWidth = 24;
            Code.Name = "Code";
            Code.ReadOnly = true;
            Code.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            Code.Width = 55;
            // 
            // Direction
            // 
            Direction.DataPropertyName = "Direction";
            Direction.HeaderText = "Direction";
            Direction.MinimumWidth = 24;
            Direction.Name = "Direction";
            Direction.ReadOnly = true;
            Direction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            Direction.Width = 75;
            // 
            // Message
            // 
            Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Message.DataPropertyName = "Message";
            Message.HeaderText = "Message";
            Message.MinimumWidth = 24;
            Message.Name = "Message";
            Message.ReadOnly = true;
            Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // PacketData
            // 
            PacketData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            PacketData.DataPropertyName = "PacketData";
            PacketData.HeaderText = "Data";
            PacketData.MinimumWidth = 24;
            PacketData.Name = "PacketData";
            PacketData.ReadOnly = true;
            PacketData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            PacketData.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1272, 714);
            Controls.Add(trafficGroup);
            Controls.Add(leftPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "MUnique OpenMU Network Analyzer";
            ((System.ComponentModel.ISupportInitialize)packetGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)packetBindingSource).EndInit();
            packetInfoGroup.ResumeLayout(false);
            packetInfoGroup.PerformLayout();
            settingsGroup.ResumeLayout(false);
            settingsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)targetPortNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)listenerPortNumericUpDown).EndInit();
            connectedClientsGroup.ResumeLayout(false);
            connectionContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)clientBindingSource).EndInit();
            trafficGroup.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Zuby.ADGV.AdvancedDataGridView packetGridView;
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
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.ContextMenuStrip connectionContextMenu;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPacketSenderStripMenuItem;
        private System.Windows.Forms.ComboBox clientVersionComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn PacketSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewTextBoxColumn PacketData;
    }
}

