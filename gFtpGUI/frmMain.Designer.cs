namespace gFtpGUI
{
    partial class frmMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtFtpServer = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblFtpServer = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.grpFtpFiles = new System.Windows.Forms.GroupBox();
            this.tlpFtpContents = new System.Windows.Forms.TableLayoutPanel();
            this.spltFtp = new System.Windows.Forms.SplitContainer();
            this.trvFtpFolders = new System.Windows.Forms.TreeView();
            this.grdFtpFiles = new gpower2.gControls.gDataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnRefreshFtpPath = new System.Windows.Forms.Button();
            this.txtFtpPath = new System.Windows.Forms.TextBox();
            this.lblFtpPath = new System.Windows.Forms.Label();
            this.grpLocalFiles = new System.Windows.Forms.GroupBox();
            this.tlpLocalContents = new System.Windows.Forms.TableLayoutPanel();
            this.spltLocal = new System.Windows.Forms.SplitContainer();
            this.trvLocalFolders = new System.Windows.Forms.TreeView();
            this.grdLocalFiles = new gpower2.gControls.gDataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRefreshLocalPath = new System.Windows.Forms.Button();
            this.txtLocalDriveInfo = new System.Windows.Forms.TextBox();
            this.cmbLocalDrives = new System.Windows.Forms.ComboBox();
            this.lblLocalDrives = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.lblLocalPath = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.grpFtpServer = new System.Windows.Forms.GroupBox();
            this.btnDeleteFtpConnection = new System.Windows.Forms.Button();
            this.btnSaveFtpConnection = new System.Windows.Forms.Button();
            this.cmbFtpConnections = new gpower2.gControls.gComboBox();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.spltFiles = new System.Windows.Forms.SplitContainer();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnDeleteRemoteFile = new System.Windows.Forms.Button();
            this.btnJobs = new System.Windows.Forms.Button();
            this.btnSaveAriaPath = new System.Windows.Forms.Button();
            this.btnBrowseAria = new System.Windows.Forms.Button();
            this.lblAria = new System.Windows.Forms.Label();
            this.txtAria = new System.Windows.Forms.TextBox();
            this.grpFtpFiles.SuspendLayout();
            this.tlpFtpContents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltFtp)).BeginInit();
            this.spltFtp.Panel1.SuspendLayout();
            this.spltFtp.Panel2.SuspendLayout();
            this.spltFtp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdFtpFiles)).BeginInit();
            this.panel3.SuspendLayout();
            this.grpLocalFiles.SuspendLayout();
            this.tlpLocalContents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltLocal)).BeginInit();
            this.spltLocal.Panel1.SuspendLayout();
            this.spltLocal.Panel2.SuspendLayout();
            this.spltLocal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdLocalFiles)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpFtpServer.SuspendLayout();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltFiles)).BeginInit();
            this.spltFiles.Panel1.SuspendLayout();
            this.spltFiles.Panel2.SuspendLayout();
            this.spltFiles.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFtpServer
            // 
            this.txtFtpServer.Location = new System.Drawing.Point(71, 20);
            this.txtFtpServer.Name = "txtFtpServer";
            this.txtFtpServer.Size = new System.Drawing.Size(424, 23);
            this.txtFtpServer.TabIndex = 0;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(567, 20);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 23);
            this.txtUsername.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(734, 20);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 23);
            this.txtPassword.TabIndex = 2;
            // 
            // lblFtpServer
            // 
            this.lblFtpServer.AutoSize = true;
            this.lblFtpServer.Location = new System.Drawing.Point(6, 24);
            this.lblFtpServer.Name = "lblFtpServer";
            this.lblFtpServer.Size = new System.Drawing.Size(61, 15);
            this.lblFtpServer.TabIndex = 3;
            this.lblFtpServer.Text = "FTP Server";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(503, 24);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(60, 15);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(673, 24);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 15);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(838, 16);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 30);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // grpFtpFiles
            // 
            this.grpFtpFiles.Controls.Add(this.tlpFtpContents);
            this.grpFtpFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFtpFiles.Location = new System.Drawing.Point(0, 0);
            this.grpFtpFiles.Name = "grpFtpFiles";
            this.grpFtpFiles.Size = new System.Drawing.Size(593, 485);
            this.grpFtpFiles.TabIndex = 11;
            this.grpFtpFiles.TabStop = false;
            this.grpFtpFiles.Text = "FTP";
            // 
            // tlpFtpContents
            // 
            this.tlpFtpContents.ColumnCount = 1;
            this.tlpFtpContents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFtpContents.Controls.Add(this.spltFtp, 0, 1);
            this.tlpFtpContents.Controls.Add(this.panel3, 0, 0);
            this.tlpFtpContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFtpContents.Location = new System.Drawing.Point(3, 19);
            this.tlpFtpContents.Name = "tlpFtpContents";
            this.tlpFtpContents.RowCount = 2;
            this.tlpFtpContents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpFtpContents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFtpContents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpFtpContents.Size = new System.Drawing.Size(587, 463);
            this.tlpFtpContents.TabIndex = 0;
            // 
            // spltFtp
            // 
            this.spltFtp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltFtp.Location = new System.Drawing.Point(3, 43);
            this.spltFtp.Name = "spltFtp";
            // 
            // spltFtp.Panel1
            // 
            this.spltFtp.Panel1.Controls.Add(this.trvFtpFolders);
            // 
            // spltFtp.Panel2
            // 
            this.spltFtp.Panel2.Controls.Add(this.grdFtpFiles);
            this.spltFtp.Size = new System.Drawing.Size(581, 417);
            this.spltFtp.SplitterDistance = 198;
            this.spltFtp.TabIndex = 9;
            // 
            // trvFtpFolders
            // 
            this.trvFtpFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvFtpFolders.Location = new System.Drawing.Point(0, 0);
            this.trvFtpFolders.Name = "trvFtpFolders";
            this.trvFtpFolders.Size = new System.Drawing.Size(198, 417);
            this.trvFtpFolders.TabIndex = 0;
            this.trvFtpFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvFtpFolders_AfterSelect);
            // 
            // grdFtpFiles
            // 
            this.grdFtpFiles.AllowUserToAddRows = false;
            this.grdFtpFiles.AllowUserToDeleteRows = false;
            this.grdFtpFiles.AllowUserToOrderColumns = true;
            this.grdFtpFiles.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdFtpFiles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdFtpFiles.AutoSelectFirstRowOnDataSourceChange = false;
            this.grdFtpFiles.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdFtpFiles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdFtpFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdFtpFiles.CustomColumnAlignments = "";
            this.grdFtpFiles.CustomColumnFormats = "";
            this.grdFtpFiles.CustomColumnNames = "";
            this.grdFtpFiles.CustomColumnSizes = "24";
            this.grdFtpFiles.CustomColumnSums = "";
            this.grdFtpFiles.CustomColumnWrapModes = "";
            this.grdFtpFiles.CustomFooterRowBackgroundColor = System.Drawing.Color.White;
            this.grdFtpFiles.CustomFooterRowForeColor = System.Drawing.Color.Black;
            this.grdFtpFiles.CustomRowAlternativeBackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.grdFtpFiles.CustomRowAlternativeForeColor = System.Drawing.Color.Black;
            this.grdFtpFiles.CustomRowBackgroundColor = System.Drawing.Color.White;
            this.grdFtpFiles.CustomRowForeColor = System.Drawing.Color.Black;
            this.grdFtpFiles.CustomRowSelectionBackgroundColor = System.Drawing.SystemColors.Highlight;
            this.grdFtpFiles.CustomRowSelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdFtpFiles.DefaultCellStyle = dataGridViewCellStyle2;
            this.grdFtpFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdFtpFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdFtpFiles.GridColor = System.Drawing.Color.Gainsboro;
            this.grdFtpFiles.KeyColumnIndex = -1;
            this.grdFtpFiles.LastClickedColumnIndex = -1;
            this.grdFtpFiles.LastClickedRowIndex = -1;
            this.grdFtpFiles.Location = new System.Drawing.Point(0, 0);
            this.grdFtpFiles.MultiSelect = false;
            this.grdFtpFiles.Name = "grdFtpFiles";
            this.grdFtpFiles.ReadOnly = true;
            this.grdFtpFiles.RowHeadersVisible = false;
            this.grdFtpFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdFtpFiles.ShowFooterRow = false;
            this.grdFtpFiles.Size = new System.Drawing.Size(379, 417);
            this.grdFtpFiles.TabIndex = 1;
            this.grdFtpFiles.DoubleClick += new System.EventHandler(this.grdFtpFiles_DoubleClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnRefreshFtpPath);
            this.panel3.Controls.Add(this.txtFtpPath);
            this.panel3.Controls.Add(this.lblFtpPath);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(581, 34);
            this.panel3.TabIndex = 10;
            // 
            // btnRefreshFtpPath
            // 
            this.btnRefreshFtpPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshFtpPath.Location = new System.Drawing.Point(549, 3);
            this.btnRefreshFtpPath.Name = "btnRefreshFtpPath";
            this.btnRefreshFtpPath.Size = new System.Drawing.Size(30, 30);
            this.btnRefreshFtpPath.TabIndex = 4;
            this.btnRefreshFtpPath.Text = "R";
            this.btnRefreshFtpPath.UseVisualStyleBackColor = true;
            this.btnRefreshFtpPath.Click += new System.EventHandler(this.btnRefreshFtpPath_Click);
            // 
            // txtFtpPath
            // 
            this.txtFtpPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFtpPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtFtpPath.Location = new System.Drawing.Point(39, 6);
            this.txtFtpPath.Name = "txtFtpPath";
            this.txtFtpPath.ReadOnly = true;
            this.txtFtpPath.Size = new System.Drawing.Size(504, 23);
            this.txtFtpPath.TabIndex = 3;
            // 
            // lblFtpPath
            // 
            this.lblFtpPath.AutoSize = true;
            this.lblFtpPath.Location = new System.Drawing.Point(6, 11);
            this.lblFtpPath.Name = "lblFtpPath";
            this.lblFtpPath.Size = new System.Drawing.Size(31, 15);
            this.lblFtpPath.TabIndex = 2;
            this.lblFtpPath.Text = "Path";
            // 
            // grpLocalFiles
            // 
            this.grpLocalFiles.Controls.Add(this.tlpLocalContents);
            this.grpLocalFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLocalFiles.Location = new System.Drawing.Point(0, 0);
            this.grpLocalFiles.Name = "grpLocalFiles";
            this.grpLocalFiles.Size = new System.Drawing.Size(668, 485);
            this.grpLocalFiles.TabIndex = 12;
            this.grpLocalFiles.TabStop = false;
            this.grpLocalFiles.Text = "LocalFiles";
            // 
            // tlpLocalContents
            // 
            this.tlpLocalContents.ColumnCount = 1;
            this.tlpLocalContents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLocalContents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpLocalContents.Controls.Add(this.spltLocal, 0, 2);
            this.tlpLocalContents.Controls.Add(this.panel1, 0, 0);
            this.tlpLocalContents.Controls.Add(this.panel2, 0, 1);
            this.tlpLocalContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLocalContents.Location = new System.Drawing.Point(3, 19);
            this.tlpLocalContents.Name = "tlpLocalContents";
            this.tlpLocalContents.RowCount = 3;
            this.tlpLocalContents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpLocalContents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpLocalContents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLocalContents.Size = new System.Drawing.Size(662, 463);
            this.tlpLocalContents.TabIndex = 1;
            // 
            // spltLocal
            // 
            this.spltLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltLocal.Location = new System.Drawing.Point(3, 83);
            this.spltLocal.Name = "spltLocal";
            // 
            // spltLocal.Panel1
            // 
            this.spltLocal.Panel1.Controls.Add(this.trvLocalFolders);
            // 
            // spltLocal.Panel2
            // 
            this.spltLocal.Panel2.Controls.Add(this.grdLocalFiles);
            this.spltLocal.Size = new System.Drawing.Size(656, 377);
            this.spltLocal.SplitterDistance = 257;
            this.spltLocal.TabIndex = 11;
            // 
            // trvLocalFolders
            // 
            this.trvLocalFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvLocalFolders.Location = new System.Drawing.Point(0, 0);
            this.trvLocalFolders.Name = "trvLocalFolders";
            this.trvLocalFolders.Size = new System.Drawing.Size(257, 377);
            this.trvLocalFolders.TabIndex = 0;
            this.trvLocalFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvLocalFolders_AfterSelect);
            // 
            // grdLocalFiles
            // 
            this.grdLocalFiles.AllowUserToAddRows = false;
            this.grdLocalFiles.AllowUserToDeleteRows = false;
            this.grdLocalFiles.AllowUserToOrderColumns = true;
            this.grdLocalFiles.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdLocalFiles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.grdLocalFiles.AutoSelectFirstRowOnDataSourceChange = false;
            this.grdLocalFiles.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdLocalFiles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdLocalFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdLocalFiles.CustomColumnAlignments = "";
            this.grdLocalFiles.CustomColumnFormats = "";
            this.grdLocalFiles.CustomColumnNames = "";
            this.grdLocalFiles.CustomColumnSizes = "24";
            this.grdLocalFiles.CustomColumnSums = "";
            this.grdLocalFiles.CustomColumnWrapModes = "";
            this.grdLocalFiles.CustomFooterRowBackgroundColor = System.Drawing.Color.White;
            this.grdLocalFiles.CustomFooterRowForeColor = System.Drawing.Color.Black;
            this.grdLocalFiles.CustomRowAlternativeBackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.grdLocalFiles.CustomRowAlternativeForeColor = System.Drawing.Color.Black;
            this.grdLocalFiles.CustomRowBackgroundColor = System.Drawing.Color.White;
            this.grdLocalFiles.CustomRowForeColor = System.Drawing.Color.Black;
            this.grdLocalFiles.CustomRowSelectionBackgroundColor = System.Drawing.SystemColors.Highlight;
            this.grdLocalFiles.CustomRowSelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdLocalFiles.DefaultCellStyle = dataGridViewCellStyle4;
            this.grdLocalFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdLocalFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdLocalFiles.GridColor = System.Drawing.Color.Gainsboro;
            this.grdLocalFiles.KeyColumnIndex = -1;
            this.grdLocalFiles.LastClickedColumnIndex = -1;
            this.grdLocalFiles.LastClickedRowIndex = -1;
            this.grdLocalFiles.Location = new System.Drawing.Point(0, 0);
            this.grdLocalFiles.MultiSelect = false;
            this.grdLocalFiles.Name = "grdLocalFiles";
            this.grdLocalFiles.ReadOnly = true;
            this.grdLocalFiles.RowHeadersVisible = false;
            this.grdLocalFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdLocalFiles.ShowFooterRow = false;
            this.grdLocalFiles.Size = new System.Drawing.Size(395, 377);
            this.grdLocalFiles.TabIndex = 0;
            this.grdLocalFiles.DoubleClick += new System.EventHandler(this.grdLocalFiles_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRefreshLocalPath);
            this.panel1.Controls.Add(this.txtLocalDriveInfo);
            this.panel1.Controls.Add(this.cmbLocalDrives);
            this.panel1.Controls.Add(this.lblLocalDrives);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(656, 34);
            this.panel1.TabIndex = 12;
            // 
            // btnRefreshLocalPath
            // 
            this.btnRefreshLocalPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshLocalPath.Location = new System.Drawing.Point(623, 2);
            this.btnRefreshLocalPath.Name = "btnRefreshLocalPath";
            this.btnRefreshLocalPath.Size = new System.Drawing.Size(30, 30);
            this.btnRefreshLocalPath.TabIndex = 5;
            this.btnRefreshLocalPath.Text = "R";
            this.btnRefreshLocalPath.UseVisualStyleBackColor = true;
            this.btnRefreshLocalPath.Click += new System.EventHandler(this.btnRefreshLocalPath_Click);
            // 
            // txtLocalDriveInfo
            // 
            this.txtLocalDriveInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalDriveInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtLocalDriveInfo.Location = new System.Drawing.Point(138, 6);
            this.txtLocalDriveInfo.Name = "txtLocalDriveInfo";
            this.txtLocalDriveInfo.ReadOnly = true;
            this.txtLocalDriveInfo.Size = new System.Drawing.Size(479, 23);
            this.txtLocalDriveInfo.TabIndex = 2;
            // 
            // cmbLocalDrives
            // 
            this.cmbLocalDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLocalDrives.FormattingEnabled = true;
            this.cmbLocalDrives.Location = new System.Drawing.Point(49, 6);
            this.cmbLocalDrives.Name = "cmbLocalDrives";
            this.cmbLocalDrives.Size = new System.Drawing.Size(74, 23);
            this.cmbLocalDrives.TabIndex = 1;
            this.cmbLocalDrives.SelectedIndexChanged += new System.EventHandler(this.cmbLocalDrives_SelectedIndexChanged);
            // 
            // lblLocalDrives
            // 
            this.lblLocalDrives.AutoSize = true;
            this.lblLocalDrives.Location = new System.Drawing.Point(3, 9);
            this.lblLocalDrives.Name = "lblLocalDrives";
            this.lblLocalDrives.Size = new System.Drawing.Size(39, 15);
            this.lblLocalDrives.TabIndex = 0;
            this.lblLocalDrives.Text = "Drives";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtLocalPath);
            this.panel2.Controls.Add(this.lblLocalPath);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(656, 34);
            this.panel2.TabIndex = 13;
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtLocalPath.Location = new System.Drawing.Point(49, 6);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.ReadOnly = true;
            this.txtLocalPath.Size = new System.Drawing.Size(604, 23);
            this.txtLocalPath.TabIndex = 1;
            // 
            // lblLocalPath
            // 
            this.lblLocalPath.AutoSize = true;
            this.lblLocalPath.Location = new System.Drawing.Point(3, 9);
            this.lblLocalPath.Name = "lblLocalPath";
            this.lblLocalPath.Size = new System.Drawing.Size(31, 15);
            this.lblLocalPath.TabIndex = 0;
            this.lblLocalPath.Text = "Path";
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(3, 614);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(1265, 144);
            this.grpLog.TabIndex = 13;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtLog.Location = new System.Drawing.Point(3, 19);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(1259, 122);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(6, 19);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(124, 29);
            this.btnDownload.TabIndex = 14;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // grpFtpServer
            // 
            this.grpFtpServer.Controls.Add(this.btnDeleteFtpConnection);
            this.grpFtpServer.Controls.Add(this.btnSaveFtpConnection);
            this.grpFtpServer.Controls.Add(this.cmbFtpConnections);
            this.grpFtpServer.Controls.Add(this.btnConnect);
            this.grpFtpServer.Controls.Add(this.lblPassword);
            this.grpFtpServer.Controls.Add(this.lblUsername);
            this.grpFtpServer.Controls.Add(this.lblFtpServer);
            this.grpFtpServer.Controls.Add(this.txtPassword);
            this.grpFtpServer.Controls.Add(this.txtUsername);
            this.grpFtpServer.Controls.Add(this.txtFtpServer);
            this.grpFtpServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFtpServer.Location = new System.Drawing.Point(3, 3);
            this.grpFtpServer.Name = "grpFtpServer";
            this.grpFtpServer.Size = new System.Drawing.Size(1265, 54);
            this.grpFtpServer.TabIndex = 15;
            this.grpFtpServer.TabStop = false;
            this.grpFtpServer.Text = "FTP Details";
            // 
            // btnDeleteFtpConnection
            // 
            this.btnDeleteFtpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteFtpConnection.Location = new System.Drawing.Point(1182, 16);
            this.btnDeleteFtpConnection.Name = "btnDeleteFtpConnection";
            this.btnDeleteFtpConnection.Size = new System.Drawing.Size(76, 30);
            this.btnDeleteFtpConnection.TabIndex = 9;
            this.btnDeleteFtpConnection.Text = "Delete";
            this.btnDeleteFtpConnection.UseVisualStyleBackColor = true;
            this.btnDeleteFtpConnection.Click += new System.EventHandler(this.btnDeleteFtpConnection_Click);
            // 
            // btnSaveFtpConnection
            // 
            this.btnSaveFtpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveFtpConnection.Location = new System.Drawing.Point(1102, 16);
            this.btnSaveFtpConnection.Name = "btnSaveFtpConnection";
            this.btnSaveFtpConnection.Size = new System.Drawing.Size(76, 30);
            this.btnSaveFtpConnection.TabIndex = 8;
            this.btnSaveFtpConnection.Text = "Save";
            this.btnSaveFtpConnection.UseVisualStyleBackColor = true;
            this.btnSaveFtpConnection.Click += new System.EventHandler(this.btnSaveFtpConnection_Click);
            // 
            // cmbFtpConnections
            // 
            this.cmbFtpConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFtpConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFtpConnections.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbFtpConnections.FormattingEnabled = true;
            this.cmbFtpConnections.KeyMember = "";
            this.cmbFtpConnections.Location = new System.Drawing.Point(924, 20);
            this.cmbFtpConnections.Name = "cmbFtpConnections";
            this.cmbFtpConnections.SelectedKey = null;
            this.cmbFtpConnections.Size = new System.Drawing.Size(172, 23);
            this.cmbFtpConnections.TabIndex = 7;
            this.cmbFtpConnections.SelectedIndexChanged += new System.EventHandler(this.cmbFtpConnections_SelectedIndexChanged);
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.grpFtpServer, 0, 0);
            this.tlpMain.Controls.Add(this.spltFiles, 0, 1);
            this.tlpMain.Controls.Add(this.grpLog, 0, 3);
            this.tlpMain.Controls.Add(this.grpActions, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpMain.Size = new System.Drawing.Size(1271, 761);
            this.tlpMain.TabIndex = 16;
            // 
            // spltFiles
            // 
            this.spltFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltFiles.Location = new System.Drawing.Point(3, 63);
            this.spltFiles.Name = "spltFiles";
            // 
            // spltFiles.Panel1
            // 
            this.spltFiles.Panel1.Controls.Add(this.grpFtpFiles);
            // 
            // spltFiles.Panel2
            // 
            this.spltFiles.Panel2.Controls.Add(this.grpLocalFiles);
            this.spltFiles.Size = new System.Drawing.Size(1265, 485);
            this.spltFiles.SplitterDistance = 593;
            this.spltFiles.TabIndex = 16;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnDeleteRemoteFile);
            this.grpActions.Controls.Add(this.btnJobs);
            this.grpActions.Controls.Add(this.btnSaveAriaPath);
            this.grpActions.Controls.Add(this.btnBrowseAria);
            this.grpActions.Controls.Add(this.lblAria);
            this.grpActions.Controls.Add(this.txtAria);
            this.grpActions.Controls.Add(this.btnDownload);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 554);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(1265, 54);
            this.grpActions.TabIndex = 17;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnDeleteRemoteFile
            // 
            this.btnDeleteRemoteFile.Location = new System.Drawing.Point(212, 19);
            this.btnDeleteRemoteFile.Name = "btnDeleteRemoteFile";
            this.btnDeleteRemoteFile.Size = new System.Drawing.Size(76, 29);
            this.btnDeleteRemoteFile.TabIndex = 20;
            this.btnDeleteRemoteFile.Text = "Delete";
            this.btnDeleteRemoteFile.UseVisualStyleBackColor = true;
            this.btnDeleteRemoteFile.Click += new System.EventHandler(this.btnDeleteRemoteFile_Click);
            // 
            // btnJobs
            // 
            this.btnJobs.Location = new System.Drawing.Point(134, 19);
            this.btnJobs.Name = "btnJobs";
            this.btnJobs.Size = new System.Drawing.Size(66, 29);
            this.btnJobs.TabIndex = 19;
            this.btnJobs.Text = "Jobs";
            this.btnJobs.UseVisualStyleBackColor = true;
            this.btnJobs.Click += new System.EventHandler(this.btnJobs_Click);
            // 
            // btnSaveAriaPath
            // 
            this.btnSaveAriaPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAriaPath.Location = new System.Drawing.Point(1176, 20);
            this.btnSaveAriaPath.Name = "btnSaveAriaPath";
            this.btnSaveAriaPath.Size = new System.Drawing.Size(83, 26);
            this.btnSaveAriaPath.TabIndex = 18;
            this.btnSaveAriaPath.Text = "Save...";
            this.btnSaveAriaPath.UseVisualStyleBackColor = true;
            this.btnSaveAriaPath.Click += new System.EventHandler(this.btnSaveAriaPath_Click);
            // 
            // btnBrowseAria
            // 
            this.btnBrowseAria.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseAria.Location = new System.Drawing.Point(1082, 20);
            this.btnBrowseAria.Name = "btnBrowseAria";
            this.btnBrowseAria.Size = new System.Drawing.Size(83, 26);
            this.btnBrowseAria.TabIndex = 17;
            this.btnBrowseAria.Text = "Browse...";
            this.btnBrowseAria.UseVisualStyleBackColor = true;
            this.btnBrowseAria.Click += new System.EventHandler(this.btnBrowseAria_Click);
            // 
            // lblAria
            // 
            this.lblAria.AutoSize = true;
            this.lblAria.Location = new System.Drawing.Point(294, 26);
            this.lblAria.Name = "lblAria";
            this.lblAria.Size = new System.Drawing.Size(61, 15);
            this.lblAria.TabIndex = 16;
            this.lblAria.Text = "Aria2 Path";
            // 
            // txtAria
            // 
            this.txtAria.AllowDrop = true;
            this.txtAria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAria.BackColor = System.Drawing.SystemColors.Window;
            this.txtAria.Location = new System.Drawing.Point(360, 22);
            this.txtAria.Name = "txtAria";
            this.txtAria.ReadOnly = true;
            this.txtAria.Size = new System.Drawing.Size(716, 23);
            this.txtAria.TabIndex = 15;
            this.txtAria.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtAria_DragDrop);
            this.txtAria.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtAria_DragEnter);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1271, 761);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Name = "frmMain";
            this.Text = "gFTP";
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.grpFtpFiles.ResumeLayout(false);
            this.tlpFtpContents.ResumeLayout(false);
            this.spltFtp.Panel1.ResumeLayout(false);
            this.spltFtp.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltFtp)).EndInit();
            this.spltFtp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdFtpFiles)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.grpLocalFiles.ResumeLayout(false);
            this.tlpLocalContents.ResumeLayout(false);
            this.spltLocal.Panel1.ResumeLayout(false);
            this.spltLocal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltLocal)).EndInit();
            this.spltLocal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdLocalFiles)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.grpFtpServer.ResumeLayout(false);
            this.grpFtpServer.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.spltFiles.Panel1.ResumeLayout(false);
            this.spltFiles.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltFiles)).EndInit();
            this.spltFiles.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtFtpServer;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblFtpServer;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox grpFtpFiles;
        private System.Windows.Forms.GroupBox grpLocalFiles;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.SplitContainer spltFtp;
        private System.Windows.Forms.SplitContainer spltLocal;
        private System.Windows.Forms.TreeView trvFtpFolders;
        private System.Windows.Forms.TreeView trvLocalFolders;
        private System.Windows.Forms.TableLayoutPanel tlpFtpContents;
        private System.Windows.Forms.TableLayoutPanel tlpLocalContents;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbLocalDrives;
        private System.Windows.Forms.Label lblLocalDrives;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.Label lblLocalPath;
        private System.Windows.Forms.TextBox txtLocalDriveInfo;
        private gpower2.gControls.gDataGridView grdLocalFiles;
        private gpower2.gControls.gDataGridView grdFtpFiles;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtFtpPath;
        private System.Windows.Forms.Label lblFtpPath;
        private System.Windows.Forms.GroupBox grpFtpServer;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.SplitContainer spltFiles;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnBrowseAria;
        private System.Windows.Forms.Label lblAria;
        private System.Windows.Forms.TextBox txtAria;
        private System.Windows.Forms.Button btnRefreshFtpPath;
        private System.Windows.Forms.Button btnRefreshLocalPath;
        private System.Windows.Forms.Button btnSaveFtpConnection;
        private gpower2.gControls.gComboBox cmbFtpConnections;
        private System.Windows.Forms.Button btnSaveAriaPath;
        private System.Windows.Forms.Button btnJobs;
        private System.Windows.Forms.Button btnDeleteFtpConnection;
        private System.Windows.Forms.Button btnDeleteRemoteFile;
    }
}

