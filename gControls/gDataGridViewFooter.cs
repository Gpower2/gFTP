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
    public partial class gDataGridViewFooter : UserControl
    {
        private gDataGridView _DataGridView = null;

        public gDataGridView DataGridView
        {
            get { return _DataGridView; }
            set { _DataGridView = value; }
        }

        private List<TextBox> _TextBoxList = new List<TextBox>();

        public gDataGridViewFooter(gDataGridView DataGridView)
        {
            _DataGridView = DataGridView;

            InitializeComponent();

            pnlMain.Margin = new Padding(0, 0, 0, 0);
            pnlMain.Padding = new Padding(0, 0, 0, 0);
            pnlData.Margin = new Padding(0, 0, 0, 0);
            pnlData.Padding = new Padding(0, 0, 0, 0);
        }

        public void UpdateSums(Boolean calculateSums)
        {
            if (!this.Visible) { return; }

            if (_DataGridView.Columns.Count != _TextBoxList.Count)
            {
                _TextBoxList.Clear();
            }

            if (_DataGridView == null
                || _DataGridView.Columns == null
                || _DataGridView.Columns.Count == 0)
            {
                this.SuspendLayout();
                this.SuspendDrawing();

                this.Parent = null;


                SetControls();

                this.ResumeLayout();
                this.ResumeDrawing();
                this.Invalidate(true);

                return;
            }

            String[] customColumnSums = _DataGridView.CustomColumnSumsArray;

            foreach (DataGridViewColumn col in _DataGridView.Columns)
            {
                TextBox newTextBox = null;

                if (_DataGridView.Columns.Count != _TextBoxList.Count)
                {
                    newTextBox = new TextBox();
                }
                else
                {
                    newTextBox = _TextBoxList[col.Index];
                }

                newTextBox.ReadOnly = true;
                newTextBox.BackColor = _DataGridView.CustomFooterRowBackgroundColor;
                newTextBox.ForeColor = _DataGridView.CustomFooterRowForeColor;
                newTextBox.Font = new Font(_DataGridView.ColumnHeadersDefaultCellStyle.Font, _DataGridView.ColumnHeadersDefaultCellStyle.Font.Style);
                newTextBox.Height = _DataGridView.ColumnHeadersHeight;
                newTextBox.TextAlign = HorizontalAlignment.Right;

                newTextBox.Visible = col.Visible;

                Object sum = null;

                Int32 dummyInt = 0;
                if (col.Index < customColumnSums.Length && Int32.TryParse(customColumnSums[col.Index].Trim(), out dummyInt) && dummyInt == 1 && calculateSums)
                {                    
                    if (col.ValueType == typeof(Int16)
                        || col.ValueType == typeof(Int32)
                        || col.ValueType == typeof(Int64)
                        || col.ValueType == typeof(Byte)
                        || col.ValueType == typeof(SByte))
                    {
                        sum = _DataGridView.Rows.OfType<DataGridViewRow>().Sum(t => (Int64)Convert.ChangeType(t.Cells[col.Index].Value, typeof(Int64)));
                    }
                    else if (col.ValueType == typeof(Single)
                        || col.ValueType == typeof(Double))
                    {
                        sum = _DataGridView.Rows.OfType<DataGridViewRow>().Sum(t => (Double)Convert.ChangeType(t.Cells[col.Index].Value, typeof(Double)));
                    }
                    else if (col.ValueType == typeof(Decimal))
                    {
                        sum = _DataGridView.Rows.OfType<DataGridViewRow>().Sum(t => (Decimal)Convert.ChangeType(t.Cells[col.Index].Value, typeof(Decimal)));
                    }
                }

                if (sum != null && calculateSums)
                {
                    newTextBox.Text = sum.ToString();
                }

                if (_DataGridView.Columns.Count != _TextBoxList.Count)
                {
                    _TextBoxList.Add(newTextBox);
                }
            }

            SetControls();
        }

        private void SetControls()
        {
            this.Parent = _DataGridView.Parent;

            if (pnlData.Height != this.Height)
            {
                pnlData.Height = this.Height;
            }

            if (pnlData.Location.X != -_DataGridView.HorizontalScrollingOffset)
            {
                pnlData.Location = new Point(-_DataGridView.HorizontalScrollingOffset, 0);
            }

            Int32 totalVisibleColumnsWidth = _DataGridView.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).Sum(c => c.Width);
            if(!_DataGridView.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).Any())
            {
                totalVisibleColumnsWidth = _DataGridView.Width;
            }

            if (pnlData.Width != totalVisibleColumnsWidth)
            {
                pnlData.Width = totalVisibleColumnsWidth;
            }

            Int32 pnlDataControlCount = pnlData.Controls.Cast<Control>().Where(c => c.GetType() == typeof(TextBox)).ToList().Count;
            Int32 txtBoxVisibleCount = _TextBoxList.Where(t => t.Visible).ToList().Count;

            if (pnlDataControlCount != txtBoxVisibleCount)
            {
                pnlData.Controls.Clear();
            }

            if (_TextBoxList.Count == 0)
            {
                TextBox newTextBox = new TextBox();
                newTextBox.ReadOnly = true;
                newTextBox.BackColor = _DataGridView.CustomFooterRowBackgroundColor;
                newTextBox.ForeColor = _DataGridView.CustomFooterRowForeColor;
                newTextBox.Font = new Font(_DataGridView.ColumnHeadersDefaultCellStyle.Font, _DataGridView.ColumnHeadersDefaultCellStyle.Font.Style);
                newTextBox.Height = _DataGridView.ColumnHeadersHeight;
                newTextBox.TextAlign = HorizontalAlignment.Right;

                newTextBox.Visible = true;

                newTextBox.Width = _DataGridView.Width;

                pnlData.Controls.Add(newTextBox);

                return;
            }

            for (Int32 i = 0; i < _TextBoxList.Count; i++)
            {
                if (_TextBoxList[i].Visible)
                {
                    Rectangle columnRectangle = _DataGridView.GetColumnDisplayRectangle(i, false);

                    if (_TextBoxList[i].Location.X != _DataGridView.HorizontalScrollingOffset + columnRectangle.X)
                    {
                        _TextBoxList[i].Location = new Point(_DataGridView.HorizontalScrollingOffset + columnRectangle.X, 0);
                    }

                    if (_TextBoxList[i].Width != columnRectangle.Width)
                    {
                        _TextBoxList[i].Width = columnRectangle.Width;
                    }

                    if (pnlDataControlCount != txtBoxVisibleCount)
                    {
                        pnlData.Controls.Add(_TextBoxList[i]);
                    }
                }
            }

            if (pnlDataControlCount != txtBoxVisibleCount)
            {
                this.Invalidate(true);
            }
        }
    }
}
