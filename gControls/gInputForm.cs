using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace gpower2.gControls
{
    public partial class gInputForm : gpower2.gControls.gForm
    {
        public gInputForm(String title = "Εισαγωγή στοιχείων...", String message = "", Boolean multiLineTextBox = true)
        {
            InitializeComponent();

            if ((string.IsNullOrEmpty(title.Trim())))
            {
                this.Text = "Εισαγωγή στοιχείων...";
            }
            else
            {
                this.Text = title;
            }

            if ((string.IsNullOrEmpty(message.Trim())))
            {
                this.lblMessage.Text = "Παρακαλώ εισάγετε τα στοιχεία.";
            }
            else
            {
                this.lblMessage.Text = message;
            }

            this.txtInsert.Multiline = multiLineTextBox;
            if (!multiLineTextBox)
            {
                this.Height = 3 + lblMessage.Height + 3 + txtInsert.Height + 8 + btnOk.Height + 15 + this.TitlebarHeight;
            }

            this.btnOk.Click += OnPressOk;
            this.btnCancel.Click += OnPressCancel;
            this.txtInsert.KeyPress += OnTxtInsertKeyPress;
        }

        private void OnTxtInsertKeyPress(object sender, KeyPressEventArgs e)
        {
            if(!txtInsert.Multiline && e.KeyChar == (Char)Keys.Enter)
            {
                btnOk.PerformClick();
            }
        }

        public bool HasInputText
        {
            get { return !(string.IsNullOrEmpty(txtInsert.Text.Trim())); }
        }

        public string InputText
        {
            get { return txtInsert.Text.Trim(); }
        }

        public string[] InputElements
        {
            get
            {
                if ((HasInputText))
                {
                    string codes = txtInsert.Text;
                    if ((codes.Contains(",")))
                    {
                        codes = codes.Replace(",", "\r\n");
                    }
                    return codes.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    return new string[] { "" };
                }
            }
        }

        public static string InputBox(string Prompt, string Title = "Εισαγωγή στοιχείων...", string DefaultResponse = "", Int32 XPos = -1, Int32 YPos = -1, Boolean multiLineText = true)
        {
            gInputForm inputForm = gInputForm.GetInputBoxForm(Prompt, Title, DefaultResponse, XPos, YPos, multiLineText);
            if ((inputForm.ShowDialog() == DialogResult.Cancel))
            {
                return null;
            }
            return inputForm.InputText;
        }

        public static string[] InputBoxElements(string Prompt, string Title = "Εισαγωγή στοιχείων...", string DefaultResponse = "", Int32 XPos = -1, Int32 YPos = -1)
        {
            gInputForm inputForm = gInputForm.GetInputBoxForm(Prompt, Title, DefaultResponse, XPos, YPos);
            if ((inputForm.ShowDialog() == DialogResult.Cancel))
            {
                return null;
            }
            return inputForm.InputElements;
        }

        private static gInputForm GetInputBoxForm(string Prompt, string Title = "Εισαγωγή στοιχείων...", string DefaultResponse = "", Int32 XPos = -1, Int32 YPos = -1, Boolean multiLineTextBox = true)
        {
            gInputForm inputForm = new gInputForm(Title, Prompt, multiLineTextBox);
            if ((XPos == -1 || YPos == -1))
            {
                inputForm.StartPosition = FormStartPosition.CenterParent;
            }
            else
            {
                inputForm.StartPosition = FormStartPosition.Manual;
                inputForm.Location = new Point(XPos, YPos);
            }
            inputForm.txtInsert.Text = DefaultResponse;
            return inputForm;
        }

        private void OnPressOk(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnPressCancel(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
