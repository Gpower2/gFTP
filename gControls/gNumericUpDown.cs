using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gpower2.gControls
{
    public partial class gNumericUpDown : UserControl
    {
        [Browsable(true)]
        public override String Text
        {
            get { return txtData.Text; }
            set { txtData.Text = value; }
        }

        [Browsable(true)]
        public Int32 Decimals
        {
            get { return txtData.Decimals; }
            set { txtData.Decimals = value; }
        }

        [Browsable(true)]
        public Decimal Value
        {
            get { return txtData.DecimalValue; }
            set {
                if (value < Minimum)
                {
                    txtData.DecimalValue = Minimum;
                }
                else if (value > Maximum)
                {
                    txtData.DecimalValue = Maximum;
                }
                else
                {
                    txtData.DecimalValue = value;
                }
                // Raise the ValueChanged event
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs() { });
                }
            }
        }

        [Browsable(true)]
        public Int32 Int32Value
        {
            get { return txtData.Int32Value; }
            set {
                if (value < Minimum)
                {
                    txtData.Int32Value = Convert.ToInt32(Minimum);
                }
                else if (value > Maximum)
                {
                    txtData.Int32Value = Convert.ToInt32(Maximum);
                }
                else
                {
                    txtData.Int32Value = value;
                }
                // Raise the ValueChanged event
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs() { });
                }
            }
        }

        [Browsable(true)]
        public Int64 Int64Value
        {
            get { return txtData.Int64Value; }
            set {
                if (value < Minimum)
                {
                    txtData.Int64Value = Convert.ToInt64(Minimum);
                }
                else if (value > Maximum)
                {
                    txtData.Int64Value = Convert.ToInt64(Maximum);
                }
                else
                {
                    txtData.Int64Value = value;
                }
                // Raise the ValueChanged event
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs() { });
                }
            }
        }

        protected Decimal _Maximum = 999m;

        [Browsable(true)]
        public Decimal Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; }
        }

        protected Decimal _Minimum = 0m;

        [Browsable(true)]
        public Decimal Minimum
        {
            get { return _Minimum; }
            set { _Minimum = value; }
        }

        protected Decimal _Increment = 1m;

        [Browsable(true)]
        public Decimal Increment
        {
            get { return _Increment; }
            set { _Increment = value; }
        }

        [Browsable(true)]
        public new Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                txtData.Font = value;
                btnDown.Font = value;
                btnUp.Font = value;
                SetSize();
            }
        }

        // Attribute applied to an event.
        [Description("Raised when the Value changes.")]
        [Browsable(true)]
        public event EventHandler ValueChanged;

        public gNumericUpDown()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            InitializeComponent();

            SetSize();

            this.Clear();
        }

        public void Clear()
        {
            txtData.Clear();
        }

        protected void gNumericUpDown_Resize(object sender, EventArgs e)
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
            if (Height != txtData.Height)
            {
                this.Height = txtData.Height;
            }
            btnDown.Height = txtData.Height;
            btnUp.Height = txtData.Height;

            btnDown.Width = txtData.Height;
            btnUp.Width = txtData.Height;

            btnDown.Location = new Point(0, 0);
            btnUp.Location = new Point(this.Width - btnUp.Width, 0);

            txtData.Location = new Point(btnDown.Location.X + btnDown.Width + 2, 0);
            txtData.Width = btnUp.Location.X - 2 - btnDown.Width - 2;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                Value -= Increment;
                if(Value < Minimum)
                {
                    Value = Minimum;
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                Value += Increment;
                if (Value > Maximum)
                {
                    Value = Maximum;
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Value Property getter returns the DecimalValue Property of txtData control
                // which will reflect the new value of txtData's Text Property
                if (Value < Minimum)
                {
                    Value = Minimum;
                    // ValueChanged will be raised by the Value Property setter
                    return;
                }
                else if(Value > Maximum)
                {
                    Value = Maximum;
                    // ValueChanged will be raised by the Value Property setter
                    return;
                }

                // Raise the ValueChanged event
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs() { });
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }
    }
}
