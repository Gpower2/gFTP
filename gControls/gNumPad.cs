using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace gpower2.gControls
{
    public partial class gNumPad : UserControl
    {
        private Control _TextBox = null;

        public Control TextBox
        {
            get { return _TextBox; }
            set { _TextBox = value; }
        }

        public gNumPad()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);

            this.DoubleBuffered = true;

            InitializeComponent();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            height = Convert.ToInt32(Convert.ToDouble(29 * width) / 18.0);
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private void SendKey(String str)
        {
            if(_TextBox != null)
            {
                _TextBox.Text += str;
            }
        }

        private void btnNumbers_Click(object sender, EventArgs e)
        {
            try
            {
                SendKey(((Control)sender).Tag.ToString());
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                if (_TextBox != null)
                {
                    if (_TextBox.Text.Length > 0)
                    {
                        _TextBox.Text = _TextBox.Text.Substring(0, _TextBox.Text.Length - 1);
                    }                    
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }
    }
}
