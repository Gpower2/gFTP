namespace gpower2.gControls
{
    partial class gSmartTextBox
    {

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
            if (disposing && (ArialBlackFont != null))
            {
                ArialBlackFont.Dispose();
            }
            if (disposing && (MarlettFont != null))
            {
                MarlettFont.Dispose();
            } 
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {
            this.txtContent = new gpower2.gControls.gTextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnOpenForm = new System.Windows.Forms.Button();
            this.cmbSearchResults = new gpower2.gControls.gComboBox();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.DataObject = null;
            this.txtContent.Decimals = 2;
            this.txtContent.DecimalValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtContent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtContent.Int32Value = 0;
            this.txtContent.Int64Value = ((long)(0));
            this.txtContent.Location = new System.Drawing.Point(0, 0);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(300, 23);
            this.txtContent.TabIndex = 0;
            this.txtContent.TextBoxType = gpower2.gControls.gTextBox.gTextBoxType.Text;
            this.txtContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            this.txtContent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContent_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(302, 0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(23, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "i";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(352, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(23, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "X";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnOpenForm
            // 
            this.btnOpenForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenForm.Location = new System.Drawing.Point(327, 0);
            this.btnOpenForm.Name = "btnOpenForm";
            this.btnOpenForm.Size = new System.Drawing.Size(23, 23);
            this.btnOpenForm.TabIndex = 2;
            this.btnOpenForm.Text = "^";
            this.btnOpenForm.UseVisualStyleBackColor = true;
            this.btnOpenForm.Click += new System.EventHandler(this.btnOpenForm_Click);
            // 
            // cmbSearchResults
            // 
            this.cmbSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSearchResults.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbSearchResults.FormattingEnabled = true;
            this.cmbSearchResults.KeyMember = "";
            this.cmbSearchResults.Location = new System.Drawing.Point(0, 0);
            this.cmbSearchResults.Name = "cmbSearchResults";
            this.cmbSearchResults.SelectedKey = null;
            this.cmbSearchResults.Size = new System.Drawing.Size(300, 23);
            this.cmbSearchResults.TabIndex = 4;
            this.cmbSearchResults.SelectedIndexChanged += new System.EventHandler(this.HiddenCombobox_SelectedIndexChanged);
            this.cmbSearchResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HiddenCombobox_KeyDown);
            // 
            // gSmartTextBox
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnOpenForm);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.cmbSearchResults);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Name = "gSmartTextBox";
            this.Size = new System.Drawing.Size(375, 23);
            this.Resize += new System.EventHandler(this.gSmartTextBox_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected gTextBox txtContent;
        protected System.Windows.Forms.Button btnSearch;
        protected System.Windows.Forms.Button btnClear;
        protected System.Windows.Forms.Button btnOpenForm;
        protected gComboBox cmbSearchResults;
        private System.ComponentModel.IContainer components = null;
    }
}
