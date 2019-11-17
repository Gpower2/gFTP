using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace gpower2.gControls
{
    public class gForm : Form
    {
        public Int32 BorderWidth
        {
            get { return Convert.ToInt32(Convert.ToDouble((this.Width - this.ClientSize.Width)) / 2.0); }
        }

        public Int32 TitlebarHeight
        {
            get { return this.Height - this.ClientSize.Height - 2 * BorderWidth; }
        }

        public gForm()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.Lavender;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // gForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Name = "gForm";
            this.ResumeLayout(false);
        }

        protected void ShowMdiChildForm(Form argForm)
        {
            // Try to find the open MDI Container form
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.IsMdiContainer)
                {
                    argForm.MdiParent = frm;
                    break;
                }
            }
            argForm.Show();
        }

        protected void SetMdiContainerBackColor(Color backColor)
        {
            if (this.IsMdiContainer)
            {
                // Loop through all of the form's controls looking
                // for the control of type MdiClient.
                foreach (Control ctl in this.Controls)
                {
                    if (ctl is MdiClient)
                    {
                        ((MdiClient)ctl).BackColor = backColor;
                    }
                }
            }
        }

        protected void ToggleControls(Control argRootControl, Boolean argStatus)
        {
            foreach (Control ctrl in argRootControl.Controls)
            {
                if (ctrl is IContainer)
                {
                    ToggleControls(ctrl, argStatus);
                }
                else
                {
                    ctrl.Enabled = argStatus;
                }
            }
        }

        /// <summary>
        /// Εμφανίζει ένα μήνυμα σφάλματος στον χρήστη
        /// </summary>
        /// <param name="errorMessage">Το μήνυμα σφάλματος</param>
        /// <param name="errorTitle">Ο τίτλος του μηνύματος σφάλματος</param>
        /// <remarks></remarks>
        public void ShowErrorMessage(string errorMessage, string errorTitle = "Προέκυψε σφάλμα!")
        {
            MessageBox.Show(this, String.Format("Προέκυψε σφάλμα!\r\n\r\n{0}" , errorMessage), errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Εμφανίζει ένα σφάλμα εξαίρεσης στον χρήστη και γράφει το StackTrace της εξαίρεσης στο Debug output
        /// </summary>
        /// <param name="errorException">Η εξαίρεση προς εμφάνιση</param>
        /// <param name="errorTitle">Ο τίτλος του μηνύματος εξαίρεσης</param>
        /// <remarks></remarks>
        public void ShowExceptionMessage(Exception errorException, string errorTitle = "Προέκυψε σφάλμα!")
        {
            Debug.WriteLine(errorException);
            ShowErrorMessage(errorException.Message);
        }

        /// <summary>
        /// Εμφανίζει ένα μήνυμα επιτυχίας στον χρήστη
        /// </summary>
        /// <param name="successMessage">Το μήνυμα επιτυχίας</param>
        /// <param name="successTitle">Ο τίτλος του μηνύματος επιτυχίας</param>
        /// <remarks></remarks>
        public void ShowSuccessMessage(string successMessage, string successTitle = "Επιτυχία!")
        {
            MessageBox.Show(this, successMessage, successTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Εμφανίζει ένα ενημερωτικό μήνυμα στον χρήστη
        /// </summary>
        /// <param name="infoMessage">Το ενημερωτικό κείμενο</param>
        /// <param name="infoTitle">Ο τίτλος του ενημερωτικού μηνύματος</param>
        /// <remarks></remarks>
        public void ShowInformationMessage(string infoMessage, string infoTitle = "Πληροφορίες")
        {
            MessageBox.Show(this, infoMessage, infoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Εμφανίζει ένα μήνυμα προειδοποίησης στον χρήστη
        /// </summary>
        /// <param name="warningMessage">Το μήνυμα προειδοποίησης</param>
        /// <param name="warningTitle">Ο τίτλος της προειδοποίησης</param>
        /// <remarks></remarks>
        public void ShowWarningMessage(string warningMessage, string warningTitle = "Προσοχή!")
        {
            MessageBox.Show(this, string.Format("Προσοχή!\r\n\r\n{0}", warningMessage), warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Εμφανίζει ένα μήνυμα ερώτησης στον χρήστη και επιστρέφει την απάντησή του (Yes, No, Cancel)
        /// </summary>
        /// <param name="questionMessage">Το κείμενο της ερώτησης</param>
        /// <param name="questionTitle">Ο τίτλος της ερώτησης</param>
        /// <param name="useCancel">Αν θα εμφανιστεί η επιλογή Cancel ή όχι</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DialogResult ShowQuestion(string questionMessage, string questionTitle = "Είστε σίγουροι;", bool useCancel = false)
        {
            return MessageBox.Show(this, questionMessage, questionTitle, useCancel ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Εμφανίζει ένα μήνυμα ερώτησης προειδοποιώντας τον χρήστη και επιστρέφει την απάντησή του (Yes, No, Cancel)
        /// </summary>
        /// <param name="questionMessage">Το μήνυμα της ερώτησης</param>
        /// <param name="questionTitle">Ο τίτλος της ερώτησης</param>
        /// <param name="useCancel">Αν θα εμφανιστεί η επιλογή Cancel ή όχι</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DialogResult ShowWarningQuestion(string questionMessage, string questionTitle = "Προσοχή!", bool useCancel = false)
        {
            return MessageBox.Show(this, questionMessage, questionTitle, useCancel ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
    } 
}
