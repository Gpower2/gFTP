namespace gFtpGUI
{
    partial class frmQueue
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnRemoveCompleted = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnChangeState = new System.Windows.Forms.Button();
            this.btnStopJobs = new System.Windows.Forms.Button();
            this.btnRunJobs = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.tlpQueue = new System.Windows.Forms.TableLayoutPanel();
            this.grpQueue = new System.Windows.Forms.GroupBox();
            this.grdQueue = new gpower2.gControls.gDataGridView();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.txtAriaData = new System.Windows.Forms.TextBox();
            this.lblAriaData = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.prgBrStatus = new System.Windows.Forms.ProgressBar();
            this.btnRemoveDeleted = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.tlpQueue.SuspendLayout();
            this.grpQueue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdQueue)).BeginInit();
            this.grpProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            this.tlpMain.Controls.Add(this.grpActions, 1, 0);
            this.tlpMain.Controls.Add(this.tlpQueue, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 511F));
            this.tlpMain.Size = new System.Drawing.Size(1484, 561);
            this.tlpMain.TabIndex = 0;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnRemoveDeleted);
            this.grpActions.Controls.Add(this.btnOpenFile);
            this.grpActions.Controls.Add(this.btnDown);
            this.grpActions.Controls.Add(this.btnUp);
            this.grpActions.Controls.Add(this.btnRemoveAll);
            this.grpActions.Controls.Add(this.btnRemoveCompleted);
            this.grpActions.Controls.Add(this.btnOpenFolder);
            this.grpActions.Controls.Add(this.btnChangeState);
            this.grpActions.Controls.Add(this.btnStopJobs);
            this.grpActions.Controls.Add(this.btnRunJobs);
            this.grpActions.Controls.Add(this.btnRemove);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(1370, 3);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(111, 555);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(10, 405);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(95, 40);
            this.btnOpenFile.TabIndex = 9;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(10, 508);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(95, 40);
            this.btnDown.TabIndex = 8;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(10, 462);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(95, 40);
            this.btnUp.TabIndex = 7;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(10, 113);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(95, 40);
            this.btnRemoveAll.TabIndex = 6;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnRemoveCompleted
            // 
            this.btnRemoveCompleted.Location = new System.Drawing.Point(10, 68);
            this.btnRemoveCompleted.Name = "btnRemoveCompleted";
            this.btnRemoveCompleted.Size = new System.Drawing.Size(95, 40);
            this.btnRemoveCompleted.TabIndex = 5;
            this.btnRemoveCompleted.Text = "Remove Completed";
            this.btnRemoveCompleted.UseVisualStyleBackColor = true;
            this.btnRemoveCompleted.Click += new System.EventHandler(this.btnRemoveCompleted_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(10, 359);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(95, 40);
            this.btnOpenFolder.TabIndex = 4;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnChangeState
            // 
            this.btnChangeState.Location = new System.Drawing.Point(10, 304);
            this.btnChangeState.Name = "btnChangeState";
            this.btnChangeState.Size = new System.Drawing.Size(95, 40);
            this.btnChangeState.TabIndex = 3;
            this.btnChangeState.Text = "Change State";
            this.btnChangeState.UseVisualStyleBackColor = true;
            this.btnChangeState.Click += new System.EventHandler(this.btnChangeState_Click);
            // 
            // btnStopJobs
            // 
            this.btnStopJobs.Location = new System.Drawing.Point(10, 258);
            this.btnStopJobs.Name = "btnStopJobs";
            this.btnStopJobs.Size = new System.Drawing.Size(95, 40);
            this.btnStopJobs.TabIndex = 2;
            this.btnStopJobs.Text = "Stop";
            this.btnStopJobs.UseVisualStyleBackColor = true;
            this.btnStopJobs.Click += new System.EventHandler(this.btnStopJobs_Click);
            // 
            // btnRunJobs
            // 
            this.btnRunJobs.Location = new System.Drawing.Point(10, 213);
            this.btnRunJobs.Name = "btnRunJobs";
            this.btnRunJobs.Size = new System.Drawing.Size(95, 40);
            this.btnRunJobs.TabIndex = 1;
            this.btnRunJobs.Text = "Run";
            this.btnRunJobs.UseVisualStyleBackColor = true;
            this.btnRunJobs.Click += new System.EventHandler(this.btnRunJobs_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(10, 22);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(95, 40);
            this.btnRemove.TabIndex = 0;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // tlpQueue
            // 
            this.tlpQueue.ColumnCount = 1;
            this.tlpQueue.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQueue.Controls.Add(this.grpQueue, 0, 0);
            this.tlpQueue.Controls.Add(this.grpProgress, 0, 1);
            this.tlpQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpQueue.Location = new System.Drawing.Point(3, 3);
            this.tlpQueue.Name = "tlpQueue";
            this.tlpQueue.RowCount = 2;
            this.tlpQueue.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQueue.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpQueue.Size = new System.Drawing.Size(1361, 555);
            this.tlpQueue.TabIndex = 2;
            // 
            // grpQueue
            // 
            this.grpQueue.Controls.Add(this.grdQueue);
            this.grpQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpQueue.Location = new System.Drawing.Point(3, 3);
            this.grpQueue.Name = "grpQueue";
            this.grpQueue.Size = new System.Drawing.Size(1355, 459);
            this.grpQueue.TabIndex = 0;
            this.grpQueue.TabStop = false;
            this.grpQueue.Text = "Queue";
            // 
            // grdQueue
            // 
            this.grdQueue.AllowUserToAddRows = false;
            this.grdQueue.AllowUserToDeleteRows = false;
            this.grdQueue.AllowUserToOrderColumns = true;
            this.grdQueue.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdQueue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdQueue.AutoSelectFirstRowOnDataSourceChange = false;
            this.grdQueue.BackgroundColor = System.Drawing.Color.White;
            this.grdQueue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdQueue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdQueue.CustomColumnAlignments = "";
            this.grdQueue.CustomColumnFormats = "";
            this.grdQueue.CustomColumnNames = "";
            this.grdQueue.CustomColumnSizes = "50|700|100|100|100|90|100|100";
            this.grdQueue.CustomColumnSums = "";
            this.grdQueue.CustomColumnWrapModes = "";
            this.grdQueue.CustomFooterRowBackgroundColor = System.Drawing.Color.White;
            this.grdQueue.CustomFooterRowForeColor = System.Drawing.Color.Black;
            this.grdQueue.CustomRowAlternativeBackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.grdQueue.CustomRowAlternativeForeColor = System.Drawing.Color.Black;
            this.grdQueue.CustomRowBackgroundColor = System.Drawing.Color.White;
            this.grdQueue.CustomRowForeColor = System.Drawing.Color.Black;
            this.grdQueue.CustomRowSelectionBackgroundColor = System.Drawing.SystemColors.Highlight;
            this.grdQueue.CustomRowSelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdQueue.DefaultCellStyle = dataGridViewCellStyle2;
            this.grdQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdQueue.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdQueue.GridColor = System.Drawing.Color.Gainsboro;
            this.grdQueue.KeyColumnIndex = -1;
            this.grdQueue.LastClickedColumnIndex = -1;
            this.grdQueue.LastClickedRowIndex = -1;
            this.grdQueue.Location = new System.Drawing.Point(3, 19);
            this.grdQueue.MultiSelect = false;
            this.grdQueue.Name = "grdQueue";
            this.grdQueue.ReadOnly = true;
            this.grdQueue.RowHeadersVisible = false;
            this.grdQueue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdQueue.ShowFooterRow = false;
            this.grdQueue.Size = new System.Drawing.Size(1349, 437);
            this.grdQueue.TabIndex = 0;
            this.grdQueue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.grdQueue_MouseDoubleClick);
            // 
            // grpProgress
            // 
            this.grpProgress.Controls.Add(this.txtAriaData);
            this.grpProgress.Controls.Add(this.lblAriaData);
            this.grpProgress.Controls.Add(this.lblStatus);
            this.grpProgress.Controls.Add(this.prgBrStatus);
            this.grpProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProgress.Location = new System.Drawing.Point(3, 468);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(1355, 84);
            this.grpProgress.TabIndex = 1;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Progress";
            // 
            // txtAriaData
            // 
            this.txtAriaData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAriaData.BackColor = System.Drawing.SystemColors.Window;
            this.txtAriaData.Location = new System.Drawing.Point(45, 55);
            this.txtAriaData.Name = "txtAriaData";
            this.txtAriaData.ReadOnly = true;
            this.txtAriaData.Size = new System.Drawing.Size(1233, 23);
            this.txtAriaData.TabIndex = 3;
            // 
            // lblAriaData
            // 
            this.lblAriaData.AutoSize = true;
            this.lblAriaData.Location = new System.Drawing.Point(11, 59);
            this.lblAriaData.Name = "lblAriaData";
            this.lblAriaData.Size = new System.Drawing.Size(28, 15);
            this.lblAriaData.TabIndex = 2;
            this.lblAriaData.Text = "Info";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(1291, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 1;
            // 
            // prgBrStatus
            // 
            this.prgBrStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgBrStatus.Location = new System.Drawing.Point(6, 19);
            this.prgBrStatus.Name = "prgBrStatus";
            this.prgBrStatus.Size = new System.Drawing.Size(1272, 27);
            this.prgBrStatus.TabIndex = 0;
            // 
            // btnRemoveDeleted
            // 
            this.btnRemoveDeleted.Location = new System.Drawing.Point(10, 159);
            this.btnRemoveDeleted.Name = "btnRemoveDeleted";
            this.btnRemoveDeleted.Size = new System.Drawing.Size(95, 40);
            this.btnRemoveDeleted.TabIndex = 10;
            this.btnRemoveDeleted.Text = "Remove Deleted Files";
            this.btnRemoveDeleted.UseVisualStyleBackColor = true;
            this.btnRemoveDeleted.Click += new System.EventHandler(this.btnRemoveDeleted_Click);
            // 
            // frmQueue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1484, 561);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmQueue";
            this.Text = "Download Queue";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmQueue_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.tlpQueue.ResumeLayout(false);
            this.grpQueue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdQueue)).EndInit();
            this.grpProgress.ResumeLayout(false);
            this.grpProgress.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox grpQueue;
        private System.Windows.Forms.GroupBox grpActions;
        private gpower2.gControls.gDataGridView grdQueue;
        private System.Windows.Forms.Button btnStopJobs;
        private System.Windows.Forms.Button btnRunJobs;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnChangeState;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnRemoveCompleted;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.TableLayoutPanel tlpQueue;
        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar prgBrStatus;
        private System.Windows.Forms.TextBox txtAriaData;
        private System.Windows.Forms.Label lblAriaData;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnRemoveDeleted;
    }
}