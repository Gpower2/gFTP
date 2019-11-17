using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gpower2.gControls
{
    public class gTextBox : TextBox
    {
        public enum gTextBoxType
        {
            Text,
            Numeric,
            Data
        }

        public static string PrepareStringForNumericParse(string argString)
        {
            if ((argString.Contains(".")))
            {
                // if it's more than one, then the '.' is definetely a thousand separator
                if ((argString.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Length > 2))
                {
                    // Remove the thousand separator and replace the decimal point separator
                    return argString.Replace(".", string.Empty).Replace(",", ".");
                }
                else if ((argString.Contains(",")))
                {
                    // if it also contains ',' check to see which is first
                    if ((argString.IndexOf(",") < argString.IndexOf(".")))
                    {
                        // if the ',' is before the '.', then the thousand separator is ','
                        // Remove the thousand separator and leave the decimal point separator
                        return argString.Replace(",", string.Empty);
                    }
                    else
                    {
                        // if the ',' is after the '.', then the thousand separator is '.'
                        // Remove the thousand separator and replace the decimal point separator
                        return argString.Replace(".", string.Empty).Replace(",", ".");
                    }
                }
                else
                {
                    // if we have only a '.' present, we assume it is a decimal point separator
                    // let it be
                    return argString;
                }
            }
            else if ((argString.Contains(",")))
            {
                // if it's more than one, then the ',' is definetely a thousand separator
                if ((argString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Length > 2))
                {
                    // Remove the thousand separator and leave the decimal point separator
                    return argString.Replace(",", string.Empty);
                }
                else
                {
                    // if we have only a ',' present, we assume it is a decimal point separator
                    // Replace the decimal point separator
                    return argString.Replace(",", ".");
                }
            }
            else
            {
                // if neither '.' or ',' are found, then return the string as is
                return argString;
            }
        }

        public static Int32 GetDecimals(Decimal argDecimal)
        {
            argDecimal = Math.Abs(argDecimal); //make sure it is positive.
            argDecimal -= (Int32)argDecimal;     //remove the integer part of the number.
            Int32 decimalPlaces = 0;
            while (argDecimal > 0)
            {
                decimalPlaces++;
                argDecimal *= 10;
                argDecimal -= (Int32)argDecimal;
            }
            return decimalPlaces;
        }

        private gTextBoxType _TextBoxType = gTextBoxType.Text;

        [Browsable(true)]
        public gTextBoxType TextBoxType
        {
            get { return _TextBoxType; }
            set { 
                _TextBoxType = value;
                switch (_TextBoxType)
                {
                    case gTextBoxType.Text:
                        break;
                    case gTextBoxType.Numeric:
                        TextAlign = HorizontalAlignment.Right;
                        break;
                    case gTextBoxType.Data:
                        break;
                    default:
                        break;
                }                
                OnTextChanged(null);
            }
        }

        private Int32 _Decimals = 2;

        [Browsable(true)]
        public Int32 Decimals
        {
            get { return _Decimals; }
            set
            {
                _Decimals = value;
                OnTextChanged(null);
            }
        }

        private Object _DataObject = null;

        [Browsable(true)]
        public Object DataObject
        {
            get { return _DataObject; }
            set
            {
                _DataObject = value;
                if (_DataObject == null)
                {
                    this.Text = String.Empty;
                }
                else
                {
                    this.Text = _DataObject.ToString();
                }
            }
        }

        [Browsable(false)]
        public Decimal DecimalValue
        {
            get
            {
                if (_TextBoxType != gTextBoxType.Numeric)
                {
                    return 0m;
                    //throw new Exception("Ο τύπος του TextBox δεν είναι Numeric! Επικοινωνήστε με τον προγραμματιστή (DecimalValue)!");
                }
                if (String.IsNullOrWhiteSpace(Text))
                {
                    return 0m;
                }
                return Decimal.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                if (_TextBoxType != gTextBoxType.Numeric)
                {
                    return;
                    //throw new Exception("Ο τύπος του TextBox δεν είναι Numeric! Επικοινωνήστε με τον προγραμματιστή (DecimalValue)!");
                }
                // If value decimals are more than the property, Round the value
                Int32 decimals = GetDecimals(value);
                if (decimals > Decimals)
                {
                    value = Decimal.Round(value, Decimals, MidpointRounding.AwayFromZero);
                }
                Text = value.ToString();
            }
        }

        [Browsable(false)]
        public Int32 Int32Value
        {
            get
            {
                if (_TextBoxType != gTextBoxType.Numeric)
                {
                    return 0;
                    //throw new Exception("Ο τύπος του TextBox δεν είναι Numeric! Επικοινωνήστε με τον προγραμματιστή (Int32Value)!");
                }
                if (String.IsNullOrWhiteSpace(Text))
                {
                    return 0;
                }
                return Int32.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                if (_TextBoxType != gTextBoxType.Numeric)
                {
                    return;
                    //throw new Exception("Ο τύπος του TextBox δεν είναι Numeric! Επικοινωνήστε με τον προγραμματιστή (Int32Value)!");
                }
                Text = value.ToString();
            }
        }

        [Browsable(false)]
        public Int64 Int64Value
        {
            get
            {
                if (_TextBoxType != gTextBoxType.Numeric)
                {
                    return 0;
                    //throw new Exception("Ο τύπος του TextBox δεν είναι Numeric! Επικοινωνήστε με τον προγραμματιστή (Int64Value)!");
                }
                if (String.IsNullOrWhiteSpace(Text))
                {
                    return 0;
                }
                return Int64.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                if (_TextBoxType != gTextBoxType.Numeric)
                {
                    return;
                    //throw new Exception("Ο τύπος του TextBox δεν είναι Numeric! Επικοινωνήστε με τον προγραμματιστή (Int64Value)!");
                }
                Text = value.ToString();
            }
        }

        public gTextBox()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            try
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    this.SelectAll();
                }
                else if (e.Control && e.KeyCode == Keys.C)
                {
                    Clipboard.SetText(this.SelectedText, TextDataFormat.UnicodeText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch (_TextBoxType)
            {
                case gTextBoxType.Text:
                    break;
                case gTextBoxType.Numeric:
                    // Κόβω τα πάντα εκτός από τα νούμερα, τα δεκαδικά σημεία "." και "," και το αρνητικό πρόσημο "-"
                    if (((!char.IsControl(e.KeyChar)) && (!char.IsDigit(e.KeyChar)) && (e.KeyChar != '.') && (e.KeyChar != ',') && (e.KeyChar != '-')))
                    {
                        e.Handled = true;
                    }
                    // Επιτρέπω μόνο ένα μείον "-"
                    if ((e.KeyChar == '-' && Text.Contains("-")))
                    {
                        e.Handled = true;
                    }
                    // Επιτρέπω μόνο ένα δεκαδικό
                    // Ελέγχω για το "."
                    if ((e.KeyChar == '.' && (Text.Contains(".") || Text.Contains(','))))
                    {
                        e.Handled = true;
                    }
                    // Ελέγχω για το ","
                    if ((e.KeyChar == ',' && (Text.Contains(".") || Text.Contains(','))))
                    {
                        e.Handled = true;
                    }
                    break;
                case gTextBoxType.Data:
                    this.ReadOnly = true;
                    if (_DataObject == null)
                    {
                        this.Text = String.Empty;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            switch (_TextBoxType)
            {
                case gTextBoxType.Text:
                    break;
                case gTextBoxType.Numeric:
                    // Αποθηκεύω τη θέση και την επιλογή του κέρσορα (caret)
                    Int32 start = SelectionStart;
                    Int32 length = SelectionLength;

                    // Προετοιμάζω το κείμενο για μετατροπή σε Decimal (με InvariantCulture)
                    Text = gTextBox.PrepareStringForNumericParse(Text);
                    // Ελέγχω αν το νούμερο μπορεί να γίνει Parse σε Decimal
                    decimal tmpDecimal = default(decimal);
                    if ((!decimal.TryParse(Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmpDecimal)))
                    {
                        Text = string.Empty;
                    }
                    else
                    {
                        // Ελέγχω αν έχει δεκαδικά, ώστε να κρατήσω μόνο τα 4 ή όσα ορίζει το σχετικό property του textBox
                        if ((Text.Contains(".")))
                        {
                            if (_Decimals > 0)
                            {
                                if ((Text.Split(new string[] { "." }, StringSplitOptions.None)[1].Length > _Decimals))
                                {
                                    // Κόβω τα δεκαδικά σε όσα ορίζει το property του textBox
                                    Text = string.Format("{0}.{1}",
                                        Text.Split(new string[] { "." }, StringSplitOptions.None)[0],
                                        Text.Split(new string[] { "." }, StringSplitOptions.None)[1].Substring(0, _Decimals));
                                }
                            }
                            else
                            {
                                Text = Text.Split(new string[] { "." }, StringSplitOptions.None)[0];
                            }
                        }
                    }

                    // Επαναφέρω τη θέση και την επιλογή του κέρσορα (caret)
                    Select(start, length);
                    break;
                case gTextBoxType.Data:
                    this.ReadOnly = true;
                    if (String.IsNullOrWhiteSpace(this.Text))
                    {
                        _DataObject = null;
                    }
                    if (_DataObject == null)
                    {
                        this.Text = String.Empty;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnReadOnlyChanged(EventArgs e)
        {
            base.OnReadOnlyChanged(e);
            switch (_TextBoxType)
            {
                case gTextBoxType.Text:
                    break;
                case gTextBoxType.Numeric:
                    break;
                case gTextBoxType.Data:
                    if (!this.ReadOnly)
                    {
                        this.ReadOnly = true;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            switch (_TextBoxType)
            {
                case gTextBoxType.Text:
                    break;
                case gTextBoxType.Numeric:
                    TextAlign = HorizontalAlignment.Right;
                    break;
                case gTextBoxType.Data:
                    break;
                default:
                    break;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // gTextBox
            // 
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ResumeLayout(false);
        }
    }
}
