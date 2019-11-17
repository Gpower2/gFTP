using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace gpower2.gControls
{
    [DefaultEvent("SmartObjectChanged")]
    public partial class gSmartTextBox : UserControl
    {
        [Browsable(true)]
        public override String Text
        {
            get { return txtContent.Text; }
            set { txtContent.Text = value; }
        }

        protected Boolean _AutoSearchWhileTyping = false;
        [Browsable(true)]
        public Boolean AutoSearchWhileTyping
        {
            get { return _AutoSearchWhileTyping; }
            set { _AutoSearchWhileTyping = value; }
        }

        protected Int32 _AutoSearchMinimumCharactersThreshold = 3;
        [Browsable(true)]
        public Int32 AutoSearchMinimumCharactersThreshold
        {
            get { return _AutoSearchMinimumCharactersThreshold; }
            set { _AutoSearchMinimumCharactersThreshold = value; }
        }

        protected Type _SmartObjectType = null;

        [Browsable(true)]
        public Type SmartObjectType
        {
            get
            {
                return _SmartObjectType;
            }
            set
            {
                if (value != null)
                {
                    if (!(value.GetInterfaces().Contains(typeof(ISmartControlSearchable))))
                    {
                        throw new Exception(String.Format("Only Types that implement ISmartControlSearchable are allowed! (value: {0})", value));
                    }
                }
                _SmartObjectType = value;
            }
        }

        [Browsable(true)]
        public new Font Font
        {
            get
            {
                return txtContent.Font;
            }
            set
            {
                base.Font = value;
                txtContent.Font = value;
                cmbSearchResults.Font = value;
                SetSize();
            }
        }

        protected Boolean _IgnoreTextChanged = false;

        protected Object _SmartObject = null;
        public Object SmartObject
        {
            get
            {
                return _SmartObject;
            }
            set
            {
                _SmartObject = value;
                if (_SmartObject == null)
                {
                    _IgnoreTextChanged = true;
                    txtContent.Clear();
                    txtContent.Font = new Font(txtContent.Font, FontStyle.Regular);
                    _IgnoreTextChanged = false;
                    btnSearch.Font = MarlettFont;
                    btnSearch.Text = "6";
                    txtContent.ReadOnly = false;
                }
                else
                {
                    _IgnoreTextChanged = true;
                    txtContent.Text = _SmartObject.ToString();
                    txtContent.Font = new Font(txtContent.Font, FontStyle.Bold);
                    _IgnoreTextChanged = false;
                    btnSearch.Font = ArialBlackFont;
                    btnSearch.Text = "i";
                    txtContent.ReadOnly = true;
                }
                // Raise the SmartObjectChanged event
                if (SmartObjectChanged != null)
                {
                    SmartObjectChanged(this, new EventArgs() { });
                }
            }
        }

        // Attribute applied to an event.
        [Description("Raised when the Smart Object changes.")]
        [Browsable(true)]
        public event EventHandler SmartObjectChanged;

        protected ToolTip _ToolTip = new ToolTip();
        protected Font MarlettFont = new Font("Marlett", 7, FontStyle.Regular);
        protected Font ArialBlackFont = new Font("Arial Black", 7, FontStyle.Regular);

        public delegate void VoidDelegate();

        public gSmartTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
           
            InitializeComponent();

            btnSearch.Font = MarlettFont;
            btnOpenForm.Font = MarlettFont;
            btnClear.Font = MarlettFont;

            btnSearch.Text = "6";
            btnOpenForm.Text = "1";
            btnClear.Text = "r";

            SetSize();

            this.Clear();
        }

        public void Clear()
        {
            Text = "";
            txtContent.Clear();
            SmartObject = null;
        }

        protected void ShowToolTip(String message)
        {
            Form controlForm = this.FindForm();
            Int32 BorderWidth = Convert.ToInt32(Convert.ToDouble((controlForm.Width - controlForm.ClientSize.Width)) / 2.0);
            Int32 TitlebarHeight = controlForm.Height - controlForm.ClientSize.Height - 2 * BorderWidth;
            _ToolTip.IsBalloon = false;
            _ToolTip.ShowAlways = false;
            _ToolTip.UseFading = true;
            _ToolTip.AutomaticDelay = 500;
            _ToolTip.AutoPopDelay = 500;
            Point locationOnForm = controlForm.PointToClient(this.Parent.PointToScreen(this.Location));
            _ToolTip.Show(message, controlForm, locationOnForm.X + btnSearch.Location.X + BorderWidth, locationOnForm.Y + btnSearch.Location.Y + btnSearch.Height + TitlebarHeight + BorderWidth, 2000);
        }

        protected void OnSearch()
        {
            if (_SmartObjectType == null)
            {
                throw new Exception("Δε βρέθηκε ο τύπος του αντικειμένου! Επικοινωνήστε με τον προγραμματιστή!");
            }
            if (String.IsNullOrWhiteSpace(txtContent.Text))
            {
                // Αν δεν έχει δοθεί κείμενο για αναζήτηση, τότε δεν κάνουμε αναζήτηση
                return;
            }
            // We search the Object
            var searchResults = (Activator.CreateInstance(_SmartObjectType) as ISmartControlSearchable).Search(txtContent.Text);
            if (searchResults.Count == 1)
            {
                SmartObject = searchResults[0];
            }
            else if (searchResults.Count > 1)
            {
                // show drop down list with results
                cmbSearchResults.BeginUpdate();
                cmbSearchResults.DataSource = searchResults;
                cmbSearchResults.DisplayMember = "DisplayMember";
                cmbSearchResults.EndUpdate();
                cmbSearchResults.DroppedDown = true;
                cmbSearchResults.SelectedIndex = 0;
                cmbSearchResults.Select();
                cmbSearchResults.Focus();
                Cursor.Current = Cursors.Default;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (_SmartObject != null)
                {
                    // if we already have a Smart Object, then we show its info tag                
                    ShowToolTip((Activator.CreateInstance(_SmartObjectType) as ISmartControlSearchable).GetTooltip(_SmartObject));
                }
                else
                {
                    OnSearch();
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected void HiddenCombobox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Focus();
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected void HiddenCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchResults.SelectedIndex != -1)
                {
                    SmartObject = cmbSearchResults.SelectedItem;
                }
                else
                {
                    SmartObject = null;
                }

                if (!cmbSearchResults.DroppedDown)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected void gSmartTextBox_Resize(object sender, EventArgs e)
        {
            try
            {
                SetSize();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void SetSize()
        {
            if (Height != txtContent.Height)
            {
                this.Height = txtContent.Height;
            }
            btnSearch.Height = txtContent.Height;
            btnOpenForm.Height = txtContent.Height;
            btnClear.Height = txtContent.Height;

            btnSearch.Width = txtContent.Height;
            btnOpenForm.Width = txtContent.Height;
            btnClear.Width = txtContent.Height;

            btnClear.Location = new Point(this.Width - btnClear.Width - 1, 0);
            btnOpenForm.Location = new Point(btnClear.Location.X - btnOpenForm.Width - 2, 0);
            btnSearch.Location = new Point(btnOpenForm.Location.X - btnSearch.Width - 2, 0);

            txtContent.Width = btnSearch.Location.X - 2;
            cmbSearchResults.Width = btnSearch.Location.X - 2;
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (AutoSearchWhileTyping)
                {
                    if (!_IgnoreTextChanged)
                    {
                        if (txtContent.Text.Trim().Length > _AutoSearchMinimumCharactersThreshold)
                        {
                            OnSearch();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                SmartObject = null;
                txtContent.Select();
                txtContent.Focus();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void txtContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (_SmartObject == null)
                {
                    if (e.KeyChar == (char)Keys.Enter)
                    {
                        OnSearch();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void btnOpenForm_Click(object sender, EventArgs e)
        {
            try
            {
                if (_SmartObjectType == null)
                {
                    throw new Exception("Δε βρέθηκε ο τύπος του αντικειμένου! Επικοινωνήστε με τον προγραμματιστή!");
                }
                (Activator.CreateInstance(_SmartObjectType) as ISmartControlSearchable).OpenSubForm(_SmartObject, this);
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            btnClear.Enabled = this.Enabled;
            btnOpenForm.Enabled = this.Enabled;
            btnSearch.Enabled = this.Enabled;
            txtContent.ReadOnly = !this.Enabled;
        }
    }
}