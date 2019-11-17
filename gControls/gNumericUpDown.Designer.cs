namespace gpower2.gControls
{
    partial class gNumericUpDown
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtData = new gpower2.gControls.gTextBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.DataObject = null;
            this.txtData.Decimals = 0;
            this.txtData.DecimalValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtData.Int32Value = 0;
            this.txtData.Int64Value = ((long)(0));
            this.txtData.Location = new System.Drawing.Point(25, 0);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(200, 23);
            this.txtData.TabIndex = 0;
            this.txtData.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtData.TextBoxType = gpower2.gControls.gTextBox.gTextBoxType.Numeric;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(0, 0);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(23, 23);
            this.btnDown.TabIndex = 1;
            this.btnDown.Text = "-";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Location = new System.Drawing.Point(227, 0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(23, 23);
            this.btnUp.TabIndex = 2;
            this.btnUp.Text = "+";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // gNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.txtData);
            this.Name = "gNumericUpDown";
            this.Size = new System.Drawing.Size(250, 23);
            this.Resize += new System.EventHandler(this.gNumericUpDown_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private gTextBox txtData;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
    }
}
