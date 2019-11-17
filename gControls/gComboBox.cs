using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace gpower2.gControls
{
    public class gComboBox : ComboBox
    {
        private String _KeyMember = "";

        public String KeyMember
        {
            get { return _KeyMember; }
            set { 
                _KeyMember = value; 
            }
        }

        public Object SelectedKey
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_KeyMember))
                {
                    return null;
                }
                if (SelectedIndex == -1)
                {
                    return null;
                }
                if (!SelectedItem.GetType().GetProperties().Any(t => t.Name == _KeyMember))
                {
                    return null;
                }
                return SelectedItem.GetType().GetProperty(_KeyMember).GetValue(SelectedItem, null);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(_KeyMember))
                {
                    return;
                }
                if (Items.Count == 0)
                {
                    return;
                }
                foreach (var item in Items)
                {
                    if (!item.GetType().GetProperties().Any(t => t.Name == _KeyMember))
                    {
                        continue;
                    }
                    if (Convert.ChangeType(item.GetType().GetProperty(_KeyMember).GetValue(item, null), value.GetType()).Equals(value))
                    {
                        SelectedItem = item;
                        break;
                    }
                }
            }
        }

        protected ContextMenuStrip _ContextMenu = new ContextMenuStrip();
        protected ToolStripMenuItem _ClearMenu = new ToolStripMenuItem("Καθάρισμα");
        protected ToolStripMenuItem _CopyMenu = new ToolStripMenuItem("Αντιγραφή");

        public gComboBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            
            InitializeComponent();

            _ContextMenu.Items.Add(_CopyMenu);
            _ContextMenu.Items.Add(_ClearMenu);
            this.ContextMenuStrip = _ContextMenu;

            _CopyMenu.Click += _CopyMenu_Click;
            _ClearMenu.Click += _ClearMenu_Click;
        }

        
        void _ClearMenu_Click(object sender, EventArgs e)
        {
            try
            {
                this.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        void _CopyMenu_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.SelectedIndex > -1)
                {
                    Clipboard.SetText(this.Text);
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // gComboBox
            // 
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Size = new System.Drawing.Size(121, 21);
            this.DropDown += new System.EventHandler(this.gComboBox_DropDown);
            this.ResumeLayout(false);
        }

        protected void AutosizeDropDownWidth()
        {
            float longestItem = 0;
            // Βρίσκει το μεγαλύτερο κείμενο των στοιχείων της λίστας, ώστε να ορίσει το μέγεθος της λίστας.
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                foreach (var item in Items)
                {
                    float itemWidth = g.MeasureString(GetItemText(item), Font).Width;
                    if (itemWidth > longestItem)
                    {
                        longestItem = itemWidth;
                    }
                }
            }
            // Αν υπάρχει ScrollBar, τότε αυξάνεται το μέγεθος κατά 15.
            if (Items.Count > MaxDropDownItems)
            {
                longestItem += 15;
            }

            // Αλλάζει το μέγεθος της λίστας του ComboBox, αλλά ποτέ δεν θα την κάνει μικρότερη από το μέγεθος του ComboBox
            DropDownWidth = Convert.ToInt32(Math.Max(longestItem, Width));
        }

        protected void gComboBox_DropDown(object sender, EventArgs e)
        {
            try
            {
                AutosizeDropDownWidth();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }
    }
}
