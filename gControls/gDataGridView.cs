using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace gpower2.gControls
{
    [DefaultEvent("SelectionChanged")]
    public class gDataGridView : DataGridView
    {
        protected FieldInfo GetEventField(Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                {
                    break;
                }

                /* Find events defined as property { add; remove; } */
                field = type.GetField(String.Format("EVENT_{0}", eventName.ToUpper()), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    break;
                }
                // Search the base type of the control
                type = type.BaseType;
            }
            return field;
        }

        protected object RemoveEventHandler(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return null;
            }
            // Find the event
            FieldInfo fi = GetEventField(this.GetType(), eventName);
            if (fi == null)
            {
                return null;
            }
            // Keep the original event handler
            Object eventHandler = fi.GetValue(this);
            // Clear the original event handler 
            fi.SetValue(this, null);
            // Return the original event handler
            return eventHandler;
        }

        protected void AddEventHandler(string eventName, object eventHandler)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }
            // Find the event
            var fi = GetEventField(this.GetType(), eventName);
            if (fi == null)
            {
                return;
            }
            // Set the original event handler
            fi.SetValue(this, eventHandler);
        }

        #region "Base Properties overrides"

        // Εδώ "κρύβω" το DataSource του DataGridView, αλλά χρησιμοποιείται "υπογείως" μέσα από αυτό το Property
        [Browsable(true)]
        [Category("Data")]
        public new Object DataSource
        {
            get
            {
                // Επιστρέφω το DataSource ως έχει
                return base.DataSource;
            }
            set
            {
                try
                {
                    _DataSourcePopulating = true;
                    // Ελέγχω αν το DataSource που θέτω είναι λίστα αντικειμένων, όχι όμως του τύπου SortableBindingList
                    if (value is IList && !(value is ISortableBindingList))
                    {
                        // Δημιουργώ το SortableBindingList<T>, όπου T ο τύπος της λίστας της τιμής (value.GetType().GetGenericArguments()[0])
                        Type genericListType = typeof(SortableBindingList<>).MakeGenericType(value.GetType().GetGenericArguments()[0]);
                        // Αρχικοποιώ το SortableBindingList<T> με όρισμα την ίδια την τιμή (value)
                        ISortableBindingList objectList = (ISortableBindingList)Activator.CreateInstance(genericListType, new object[] { value });
                        // Θέτω το SortableBindingList<T> στο base DataSource property
                        base.DataSource = objectList;
                    }
                    else
                    {
                        // Αν δεν έχω λίστα αντικειμένων, αλλά οποιαδήποτε άλλη τιμή (null, Array, DataTable)
                        // τότε θέτω την τιμή ως έχει στο base DataSource property
                        base.DataSource = value;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _DataSourcePopulating = false;
                }
            }
        }

        #endregion

        protected gDataGridViewFooter _FooterRow = null;
        protected PropertyGridForm _PropertyForm = null;
        protected bool _DataSourcePopulating = false;
        protected bool _DataSourceSorting = false;

        protected Boolean _ShowFooterRow = false;

        [Browsable(true)]
        [Category("Custom Properties")]
        public bool AutoSelectFirstRowOnDataSourceChange
        {
            get; set;
        } = false;

        [Browsable(true)]
        [Category("Custom Properties")]
        public Boolean ShowFooterRow
        {
            get { return _ShowFooterRow; }
            set {
                Boolean changed = (_ShowFooterRow != value);
                _ShowFooterRow = value;
                SetFooterRow(changed, this.Width, this.Height, true);
            }
        }

        protected String _CustomColumnNames = String.Empty;

        [Browsable(true)]
        [Category("Custom Properties")]
        public String CustomColumnNames
        {
            get { return _CustomColumnNames; }
            set { _CustomColumnNames = value; SetCustomColumnProperties(); }
        }

        protected String[] _CustomColumnNamesArray
        {
            get
            {
                return GetStringArray(_CustomColumnNames, "|");
            }
        }

        protected String _CustomColumnFormats = String.Empty;

        [Browsable(true)]
        [Category("Custom Properties")]
        public String CustomColumnFormats
        {
            get { return _CustomColumnFormats; }
            set { _CustomColumnFormats = value; SetCustomColumnProperties(); }
        }

        protected String[] _CustomColumnFormatsArray
        {
            get
            {
                return GetStringArray(_CustomColumnFormats, "|");
            }
        }

        protected String _CustomColumnSizes = String.Empty;

        [Browsable(true)]
        [Category("Custom Properties")]
        public String CustomColumnSizes
        {
            get { return _CustomColumnSizes; }
            set { _CustomColumnSizes = value; SetCustomColumnProperties(); }
        }

        protected String[] _CustomColumnSizesArray
        {
            get
            {
                return GetStringArray(_CustomColumnSizes, "|");
            }
        }

        protected String _CustomColumnWrapModes = String.Empty;

        [Browsable(true)]
        [Category("Custom Properties")]
        public String CustomColumnWrapModes
        {
            get { return _CustomColumnWrapModes; }
            set { _CustomColumnWrapModes = value; SetCustomColumnProperties(); }
        }

        protected String[] _CustomColumnWrapModesArray
        {
            get
            {
                return GetStringArray(_CustomColumnWrapModes, "|");
            }
        }

        protected String _CustomColumnAlignments = String.Empty;

        [Browsable(true)]
        [Category("Custom Properties")]
        public String CustomColumnAlignments
        {
            get { return _CustomColumnAlignments; }
            set { _CustomColumnAlignments = value; SetCustomColumnProperties(); }
        }

        protected String[] _CustomColumnAlignmentsArray
        {
            get
            {
                return GetStringArray(_CustomColumnAlignments, "|");
            }
        }

        protected String _CustomColumnSums = String.Empty;

        [Browsable(true)]
        [Category("Custom Properties")]
        public String CustomColumnSums
        {
            get { return _CustomColumnSums; }
            set { _CustomColumnSums = value; SetFooterRow(false, this.Width, this.Height, true); }
        }

        public String[] CustomColumnSumsArray
        {
            get
            {
                return GetStringArray(_CustomColumnSums, "|");
            }
        }

        protected Color _CustomRowBackgroundColor = Color.White;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomRowBackgroundColor
        {
            get { return _CustomRowBackgroundColor; }
            set { _CustomRowBackgroundColor = value; SetCustomColumnProperties(); }
        }

        protected Color _CustomRowForeColor = Color.Black;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomRowForeColor
        {
            get { return _CustomRowForeColor; }
            set { _CustomRowForeColor = value; SetCustomColumnProperties(); }
        }

        protected Color _CustomRowSelectionBackgroundColor = SystemColors.Highlight;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomRowSelectionBackgroundColor
        {
            get { return _CustomRowSelectionBackgroundColor; }
            set { _CustomRowSelectionBackgroundColor = value; SetCustomColumnProperties(); }
        }

        protected Color _CustomRowSelectionForeColor = SystemColors.HighlightText;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomRowSelectionForeColor
        {
            get { return _CustomRowSelectionForeColor; }
            set { _CustomRowSelectionForeColor = value; SetCustomColumnProperties(); }
        }

        protected Color _CustomRowAlternativeBackgroundColor = Color.MintCream;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomRowAlternativeBackgroundColor
        {
            get { return _CustomRowAlternativeBackgroundColor; }
            set { _CustomRowAlternativeBackgroundColor = value; SetCustomColumnProperties(); }
        }

        protected Color _CustomRowAlternativeForeColor = Color.Black;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomRowAlternativeForeColor
        {
            get { return _CustomRowAlternativeForeColor; }
            set { _CustomRowAlternativeForeColor = value; SetCustomColumnProperties(); }
        }


        protected Color _CustomFooterRowBackgroundColor = Color.White;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomFooterRowBackgroundColor
        {
            get { return _CustomFooterRowBackgroundColor; }
            set { _CustomFooterRowBackgroundColor = value; SetFooterRow(false, this.Width, this.Height, false); }
        }

        protected Color _CustomFooterRowForeColor = Color.Black;

        [Browsable(true)]
        [Category("Appearance")]
        public Color CustomFooterRowForeColor
        {
            get { return _CustomFooterRowForeColor; }
            set { _CustomFooterRowForeColor = value; SetFooterRow(false, this.Width, this.Height, false); }
        }

        [Browsable(false)]
        public object SelectedItem
        {
            get
            {
                if (this.SelectedRows.Count > 0)
                {
                    return this.SelectedRows[0].DataBoundItem;
                }
                else
                {
                    return null;
                }
            }
        }

        [Browsable(false)]
        public IList SelectedItems
        {
            get
            {
                if (this.SelectedRows.Count > 0)
                {
                    return this.SelectedRows.Cast<DataGridViewRow>().Select(d =>d.DataBoundItem).ToList();
                }
                else
                {
                    return new List<Object>();
                }
            }
        }

        [Browsable(false)]
        public Int32 SelectedIndex
        {
            get
            {
                if (this.SelectedRows.Count > 0)
                {
                    return this.SelectedRows[0].Index;
                }
                else
                {
                    return -1;
                }
            }
        }

        protected Int32 _KeyColumnIndex = -1;

        [Browsable(true)]
        public Int32 KeyColumnIndex
        {
            get { return _KeyColumnIndex; }
            set { _KeyColumnIndex = value; }
        }

        protected Int32 _LastClickedRowIndex = -1;

        [Browsable(false)]
        public Int32 LastClickedRowIndex
        {
            get { return _LastClickedRowIndex; }
            set { _LastClickedRowIndex = value; }
        }

        protected Int32 _LastClickedColumnIndex = -1;

        [Browsable(false)]
        public Int32 LastClickedColumnIndex
        {
            get { return _LastClickedColumnIndex; }
            set { _LastClickedColumnIndex = value; }
        }

        //Ορίζω το default intformat
        protected string _DefaultIntFormat = "#,##0";
        //Ορίζω το default dateformat
        protected string _DefaultDateFormat = "dd/MM/yyyy";
        //Ορίζω το default format για αριθμούς double, decimal και single
        protected string _DefaultDecimalFormat = "#,###.00";
        protected CultureInfo _GreekCulture = CultureInfo.GetCultureInfo("el-GR", "el-GR");

        public gDataGridView()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            InitializeComponent();

            SetCustomColumnProperties();
            this.GridColor = Color.Gainsboro;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            BindContextMenu();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(_PropertyForm!= null)
            {                
                if (!_PropertyForm.IsDisposed)
                {
                    _PropertyForm.Close();
                }
                _PropertyForm = null;
            }
        }

        public void SetSelectedRowByIndex(Int32 idx)
        {
            // check if DataGridView contains any rows
            if (this.Rows.Count == 0)
            {
                return;
            }
            // check if index is greater than the last row index
            if (idx >= this.Rows.Count - 1)
            {
                idx = this.Rows.Count - 1;
            }
            // check if index is less than 0
            if (idx < 0)
            {
                idx = 0;
            }

            if (KeyColumnIndex > -1 && KeyColumnIndex < this.ColumnCount)
            {
                _LastSelectedKey = this.Rows[idx].Cells[0].Value;
            }

            this.Rows[idx].Selected = true;
            // Try to find the first visible cell
            Int32 firsVisibleCellIndex = this.Rows[idx].Cells.Cast<DataGridViewCell>().Where(c => c.Visible).FirstOrDefault()?.ColumnIndex ?? 0;
            this.CurrentCell = this.Rows[idx]?.Cells[firsVisibleCellIndex];
            this.FirstDisplayedScrollingRowIndex = idx;
            this.PerformLayout();
        }

        public void SetSelectedRowByKey(object key)
        {
            // check if DataGridView contains any rows
            if (this.Rows.Count == 0)
            {
                return;
            }
            // check if KeyColumnIndex is set
            if (KeyColumnIndex == -1)
            {
                return;
            }
            // check if KeyColumnIndex is valid
            if (KeyColumnIndex >= this.ColumnCount)
            {
                return;
            }
            DataGridViewRow keyRow = null;
            foreach (DataGridViewRow row in this.Rows)
            {
                try
                {
                    if (Convert.ChangeType(row.Cells[KeyColumnIndex].Value, key.GetType()).Equals(key))
                    {
                        keyRow = row;
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }

            Int32 keyIndex = -1;
            if (keyRow != null)
            {
                keyIndex = keyRow.Index;
            }
            if (keyIndex > -1)
            {
                _LastSelectedKey = key;
                this.Rows[keyIndex].Selected = true;
                //this.CurrentCell = this.Rows[keyIndex].Cells[0];
                this.FirstDisplayedScrollingRowIndex = keyIndex;
                this.PerformLayout();
            }
        }

        protected void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // gDataGridView
            // 
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToOrderColumns = true;
            this.AllowUserToResizeRows = false;
            this.BackgroundColor = System.Drawing.Color.GhostWhite;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DefaultCellStyle = dataGridViewCellStyle2;
            this.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.MultiSelect = false;
            this.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.RowHeadersVisible = false;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.gDataGridView_CellContextMenuStripNeeded);
            this.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gDataGridView_CellMouseDown);
            this.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gDataGridView_CellMouseMove);
            this.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gDataGridView_CellMouseUp);
            this.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gDataGridView_DataBindingComplete);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.gDataGridView_Scroll);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        protected String[] GetStringArray(String argStringToSplit, String argSplitSeparator)
        {
            return String.IsNullOrWhiteSpace(argStringToSplit) ? new String[] { } : argStringToSplit.Split(new String[] { argSplitSeparator }, StringSplitOptions.None);
        }

        protected void SetCustomColumnProperties()
        {
            try
            {
                this.SuspendLayout();

                this.DefaultCellStyle.BackColor = _CustomRowBackgroundColor;
                this.DefaultCellStyle.ForeColor = _CustomRowForeColor;
                this.DefaultCellStyle.SelectionBackColor = _CustomRowSelectionBackgroundColor;
                this.DefaultCellStyle.SelectionForeColor = _CustomRowSelectionForeColor;
                this.AlternatingRowsDefaultCellStyle.BackColor = _CustomRowAlternativeBackgroundColor;
                this.AlternatingRowsDefaultCellStyle.ForeColor = _CustomRowAlternativeForeColor;
                this.AlternatingRowsDefaultCellStyle.SelectionBackColor = _CustomRowSelectionBackgroundColor;
                this.AlternatingRowsDefaultCellStyle.SelectionForeColor = _CustomRowSelectionForeColor;

                if (!String.IsNullOrWhiteSpace(_CustomColumnNames)
                    || !String.IsNullOrWhiteSpace(_CustomColumnFormats)
                    || !String.IsNullOrWhiteSpace(_CustomColumnSizes)
                    || !String.IsNullOrWhiteSpace(_CustomColumnWrapModes)
                    || !String.IsNullOrWhiteSpace(_CustomColumnAlignments))
                {
                    String[] customColumnNames = _CustomColumnNamesArray;
                    String[] customColumnFormats = _CustomColumnFormatsArray;
                    String[] customColumnSizes = _CustomColumnSizesArray;
                    String[] customColumnWrapModes = _CustomColumnWrapModesArray;
                    String[] customColumnAlignments = _CustomColumnAlignmentsArray;

                    Int32 dummyInt32 = -1;
                    for (Int32 i = 0; i < this.ColumnCount; i++)
                    {
                        if (i < customColumnNames.Length)
                        {
                            if (!String.IsNullOrWhiteSpace(customColumnNames[i]))
                            {
                                this.Columns[i].HeaderText = customColumnNames[i];
                            }
                        }
                        if (i < customColumnFormats.Length)
                        {
                            if (!String.IsNullOrWhiteSpace(customColumnFormats[i]))
                            {
                                this.Columns[i].DefaultCellStyle.Format = customColumnFormats[i];
                            }
                        }
                        if (i < customColumnWrapModes.Length)
                        {
                            if (String.IsNullOrWhiteSpace(customColumnWrapModes[i]))
                            {
                                this.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.NotSet;
                            }
                            else if (customColumnWrapModes[i] == "1")
                            {
                                this.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                            }
                            else if (customColumnWrapModes[i] == "0")
                            {
                                this.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                            }
                            else
                            {
                                this.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.NotSet;
                            }
                        }
                        if (i < customColumnSizes.Length)
                        {
                            if (String.IsNullOrWhiteSpace(customColumnSizes[i])
                                || customColumnSizes[i] == "-")
                            {
                                this.Columns[i].Visible = false;
                            }
                            else
                            {
                                this.Columns[i].Visible = true;
                                if (customColumnSizes[i] == "*")
                                {
                                    this.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                }
                                else if (customColumnSizes[i] == "=")
                                {
                                    this.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                }
                                else if (Int32.TryParse(customColumnSizes[i], out dummyInt32))
                                {
                                    this.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                                    this.Columns[i].Width = dummyInt32;
                                }
                                else
                                {
                                    this.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                                }
                            }
                        }
                        if (i < customColumnAlignments.Length)
                        {
                            if (String.IsNullOrWhiteSpace(customColumnAlignments[i]))
                            {
                                this.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.NotSet;
                                this.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.NotSet;
                            }
                            else
                            {
                                if (customColumnAlignments[i].Length > 0)
                                {
                                    // Header Alignments
                                    String headerAlignment = customColumnAlignments[i][0].ToString();
                                    if (headerAlignment == "<")
                                    {
                                        this.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                    }
                                    else if (headerAlignment == ">")
                                    {
                                        this.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    }
                                    else if (headerAlignment == "-")
                                    {
                                        this.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                    }
                                    else
                                    {
                                        this.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.NotSet;
                                    }

                                    // Check for row Alignments
                                    if (customColumnAlignments[i].Length > 1)
                                    {
                                        String rowAlignment = customColumnAlignments[i][1].ToString();
                                        if (rowAlignment == "<")
                                        {
                                            this.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                        }
                                        else if (rowAlignment == ">")
                                        {
                                            this.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                        }
                                        else if (rowAlignment == "-")
                                        {
                                            this.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                        }
                                        else
                                        {
                                            this.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.NotSet;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.ResumeLayout();
                this.PerformLayout();
            }
        }

        void _ContextMenu_Popup(object sender, EventArgs e)
        {
            try
            {
                BuildContextMenu();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected void gDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                this.SuspendLayout();
                if (_KeyColumnIndex > -1 && _LastSelectedKey != null && e.ListChangedType == ListChangedType.Reset)
                {
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        this.SetSelectedRowByKey(_LastSelectedKey);
                    });
                }
                SetCustomColumnProperties();
                SetFooterRow(false, this.Width, this.Height, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        #region "Context Menu"

        protected ContextMenuStrip _ContextMenu = new ContextMenuStrip();

        protected ToolStripMenuItem _OpenPropertyForm = new ToolStripMenuItem("Άνοιγμα Φόρμας Ιδιοτήτων");
        protected ToolStripMenuItem _CopyGridProperties = new ToolStripMenuItem("Αντιγραφή Ιδιοτήτων");
        protected ToolStripMenuItem _CopyGridContents = new ToolStripMenuItem("Αντιγραφή");
        protected ToolStripMenuItem _CopyCellContent = new ToolStripMenuItem("Αντιγραφή Κελιού");
        protected ToolStripMenuItem _CopyColumnContent = new ToolStripMenuItem("Αντιγραφή Στήλης");
        protected ToolStripMenuItem _CopySelectionContent = new ToolStripMenuItem("Αντιγραφή Επιλογής");
        protected ToolStripMenuItem _ExportGridContent = new ToolStripMenuItem("Εξαγωγή σε Excel (*.xlsx)");
        protected ToolStripMenuItem _ClearSelection = new ToolStripMenuItem("Καθάρισμα Επιλογής");
        protected ToolStripMenuItem _SelectAll = new ToolStripMenuItem("Επιλογή Όλων");

        protected void BindContextMenu()
        {
            this.ContextMenuStrip = _ContextMenu;
            BuildContextMenu();

            this.ContextMenuStrip.Opening += _ContextMenu_Popup;

            _CopyGridProperties.Click += _CopyGridProperties_Click;
            _OpenPropertyForm.Click += _OpenPropertyForm_Click;
            _CopyGridContents.Click += _CopyGridContents_Click;
            _CopyCellContent.Click += _CopyCellContent_Click;
            _CopyColumnContent.Click += _CopyColumnContent_Click;
            _CopySelectionContent.Click += _CopySelectionContent_Click;
            _ExportGridContent.Click += _ExportGridContents_Click;
            _SelectAll.Click += _SelectAll_Click;
            _ClearSelection.Click += _ClearSelection_Click;
        }

        protected void BuildContextMenu()
        {
            _ContextMenu.Items.Clear();

            // Start adding the Menu Items
            _ContextMenu.Items.Add(_CopyGridContents);
            _ContextMenu.Items.Add(_CopyCellContent);
            _ContextMenu.Items.Add(_CopyColumnContent);
            _CopySelectionContent.Text = String.Format("Αντιγραφή Επιλογής ({0})", this.SelectedRows != null ? this.SelectedRows.Count : 0);
            _ContextMenu.Items.Add(_CopySelectionContent);
            if (this.SelectedIndex == -1)
            {
                _ContextMenu.Items[_ContextMenu.Items.Count - 1].Enabled = false;
            }
            else
            {
                _ContextMenu.Items[_ContextMenu.Items.Count - 1].Enabled = true;
            }
            _ContextMenu.Items.Add("-");
            _SelectAll.Text = String.Format("Επιλογή Όλων ({0})", this.Rows.Count);
            _ContextMenu.Items.Add(_SelectAll);
            if (!MultiSelect)
            {
                _ContextMenu.Items[_ContextMenu.Items.Count - 1].Enabled = false;
            }
            else
            {
                _ContextMenu.Items[_ContextMenu.Items.Count - 1].Enabled = true;
            }
            _ContextMenu.Items.Add(_ClearSelection);
            if (SelectedIndex == -1)
            {
                _ContextMenu.Items[_ContextMenu.Items.Count - 1].Enabled = false;
            }
            else
            {
                _ContextMenu.Items[_ContextMenu.Items.Count - 1].Enabled = true;
            }

            _ContextMenu.Items.Add("-");
            _ContextMenu.Items.Add(_ExportGridContent);

            _ContextMenu.Items.Add("-");
            _ContextMenu.Items.Add(_OpenPropertyForm);
            _ContextMenu.Items.Add(_CopyGridProperties);
        }

        private void _ClearSelection_Click(object sender, EventArgs e)
        {
            this.ClearSelection();
        }

        private void _SelectAll_Click(object sender, EventArgs e)
        {
            if (MultiSelect)
            {
                this.SelectAll();
            }
        }

        private void _CopyGridProperties_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.Columns != null))
                {
                    StringBuilder propBuilder = new StringBuilder();
                    propBuilder.AppendFormat("Column Count: {0}\r\n", Columns.Count);
                    propBuilder.Append("Column Real Widths: ");
                    foreach (DataGridViewColumn col in Columns)
                    {
                        propBuilder.AppendFormat("{0}|", col.Width);
                    }
                    propBuilder.Length -= 1;
                    propBuilder.AppendLine();
                    propBuilder.Append("Column Real Names: ");
                    foreach (DataGridViewColumn col in Columns)
                    {
                        propBuilder.AppendFormat("{0}|", col.HeaderText);
                    }
                    propBuilder.Length -= 1;
                    propBuilder.AppendLine();
                    propBuilder.AppendFormat("Column Sizes: {0}", CustomColumnSizes);
                    propBuilder.AppendLine();
                    propBuilder.AppendFormat("Column Formats: {0}", CustomColumnFormats);
                    propBuilder.AppendLine();
                    propBuilder.AppendFormat("Column Names: {0}", CustomColumnNames);
                    propBuilder.AppendLine();
                    propBuilder.AppendFormat("Column Alignments: {0}", CustomColumnAlignments);
                    propBuilder.AppendLine();
                    //propBuilder.AppendFormat("Column Sums: {0}", _ColSums);
                    //propBuilder.AppendLine();
                    //propBuilder.AppendFormat("Column Null Values: {0}", _ColNullValues);
                    //propBuilder.AppendLine();
                    //propBuilder.AppendFormat("Column Color: {0}", _ColorCol);
                    Clipboard.SetDataObject(propBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ex.ShowException();
            }
        }

        private void _OpenPropertyForm_Click(object sender, EventArgs e)
        {
            try
            {
                Form parentForm = this.FindForm();
                if(_PropertyForm == null)
                {
                    _PropertyForm = new PropertyGridForm();
                    _PropertyForm.FormClosed += _PropertyForm_FormClosed;
                    if (parentForm.IsMdiChild)
                    {
                        _PropertyForm.MdiParent = parentForm.MdiParent;
                    }
                    _PropertyForm.PropertyGrid.SelectedObject = this;
                    _PropertyForm.Text = String.Format("Ιδιότητες: {0}", this.Name);
                }
                _PropertyForm.Show();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void _PropertyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _PropertyForm.FormClosed -= _PropertyForm_FormClosed;
            _PropertyForm.Dispose();
            _PropertyForm = null;
        }

        private void _CopyGridContents_Click(object sender, EventArgs e)
        {
            try
            {
                //Έλεγχος αν υπάρχουν δεδομένα στο grid
                if (this.Rows.Count == 0)
                {
                    Clipboard.SetDataObject("");
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                    //Ορίζουμε τους String Builder γραμμής και τελικού κειμένου
                    System.Text.StringBuilder textBuilder = new System.Text.StringBuilder();
                    System.Text.StringBuilder lineBuilder = new System.Text.StringBuilder();
                    //Παίρνουμε τα User Formats από το grid
                    String[] customColumnFormats = _CustomColumnFormatsArray;
                    //Φτιάχνουμε τη γραμμή με τα headers
                    foreach (DataGridViewColumn dtColumn in Columns)
                    {
                        //Μόνο αν είναι visible το Column
                        if (dtColumn.Visible)
                        {
                            //Αντικαθιστώ τους ειδικούς χαρακτήρες με κενά
                            lineBuilder.Append(RemoveSpecialChars(dtColumn.HeaderText) + "\t");
                        }
                    }
                    //Αφαιρούμε τον τελευταίο χαρακτήρα TAB
                    lineBuilder.Length -= 1;
                    //Προσθέτω τη γραμμή στο τελικό κείμενο
                    textBuilder.AppendLine(lineBuilder.ToString());
                    //Φτιάχνω τις γραμμές για τα δεδομένα του κάθε row του DataTable
                    foreach (DataGridViewRow dtRow in Rows)
                    {
                        //Μηδενίζω τον StringBuilder της κάθε σειράς
                        lineBuilder.Length = 0;
                        //Για κάθε column
                        for (Int32 i = 0; i <= Columns.Count - 1; i++)
                        {
                            //Μόνο αν είναι visible το Column
                            if (Columns[i].Visible)
                            {
                                object dtObj = dtRow.Cells[i].Value;
                                if (i < customColumnFormats.Length)
                                {
                                    lineBuilder.Append(dtObj.GetClipboardTextFromProperty(_GreekCulture, customColumnFormats[i], customColumnFormats[i], customColumnFormats[i]) + "\t");
                                }
                                else
                                {
                                    lineBuilder.Append(dtObj.GetClipboardTextFromProperty(_GreekCulture, _DefaultIntFormat, _DefaultDecimalFormat, _DefaultDateFormat) + "\t");
                                }
                            }
                        }
                        //Αφαιρούμε το τελευταίο TAB
                        lineBuilder.Length -= 1;
                        //Προσθέτουμε τη γραμμή στο τελικό κείμενο
                        textBuilder.AppendLine(lineBuilder.ToString());
                    }
                    //Αφαιρούμε την τελευταία αλλαγή γραμμής (vbCrLf = CR LF 2 χαρακτήρες)
                    textBuilder.Length -= 2;
                    //Τοποθετώ το τελικό κείμενο στο Clipboard
                    Clipboard.SetDataObject(textBuilder.ToString());
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Cursor.Current = Cursors.Default;
                ex.ShowException();
            }
        }

        private void _CopyCellContent_Click(object sender, EventArgs e)
        {
            try
            {
                //Έλεγχος αν υπάρχουν δεδομένα στο grid
                if (this.Rows.Count == 0)
                {
                    Clipboard.SetDataObject(string.Empty);
                }
                else
                {
                    //Ελέγχω αν υπάρχει κρατημένη επιλογή
                    if ((LastClickedRowIndex > -1 && LastClickedColumnIndex > -1))
                    {
                        //Παίρνουμε τα User Formats από το grid
                        String[] customColumnFormats = _CustomColumnFormatsArray;
                        string textForClipboard = string.Empty;
                        object dtObj = this.Rows[LastClickedRowIndex].Cells[LastClickedColumnIndex].Value;

                        if (LastClickedColumnIndex < customColumnFormats.Length)
                        {
                            textForClipboard = dtObj.GetClipboardTextFromProperty(_GreekCulture, customColumnFormats[LastClickedColumnIndex], customColumnFormats[LastClickedColumnIndex], customColumnFormats[LastClickedColumnIndex]);
                        }
                        else
                        {
                            textForClipboard = dtObj.GetClipboardTextFromProperty(_GreekCulture, _DefaultIntFormat, _DefaultDecimalFormat, _DefaultDateFormat);
                        }
                        //Τοποθετώ το τελικό κείμενο στο Clipboard
                        Clipboard.SetDataObject(textForClipboard);
                    }
                    else
                    {
                        Clipboard.SetDataObject(string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Cursor.Current = Cursors.Default;
                ex.ShowException();
            }
        }

        private void _CopyColumnContent_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Έλεγχος αν υπάρχουν δεδομένα στο grid
                if (this.Rows.Count == 0)
                {
                    Clipboard.SetDataObject(string.Empty);
                }
                else
                {
                    //Ελέγχω αν υπάρχει κρατημένη επιλογή
                    if ((LastClickedRowIndex > -1 && LastClickedColumnIndex > -1))
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        //Ορίζουμε τους String Builder γραμμής και τελικού κειμένου
                        System.Text.StringBuilder textBuilder = new System.Text.StringBuilder();
                        //Παίρνουμε τα User Formats από το grid
                        String[] customColumnFormats = _CustomColumnFormatsArray;
                        string separator = string.Empty;
                        if (MessageBox.Show("Να μετατραπεί η στήλη σε γραμμή με διαχωριστικό κόμμα;", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            separator = ", ";
                        }
                        else
                        {
                            separator = "\r\n";
                        }

                        //Παίρνω το κείμενο για τα δεδομένα της επιλεγμένης στήλης του κάθε row του DataTable
                        foreach (DataGridViewRow dtRow in this.Rows)
                        {
                            object dtObj = dtRow.Cells[LastClickedColumnIndex].Value;
                            //Προσθέτουμε το κείμενο της επιλεγμένης στήλης στο τελικό κείμενο
                            if (LastClickedColumnIndex < customColumnFormats.Length)
                            {
                                textBuilder.Append(RemoveSpecialChars(dtObj.GetClipboardTextFromProperty(_GreekCulture, customColumnFormats[LastClickedColumnIndex], customColumnFormats[LastClickedColumnIndex], customColumnFormats[LastClickedColumnIndex])) + separator);
                            }
                            else
                            {
                                textBuilder.Append(RemoveSpecialChars(dtObj.GetClipboardTextFromProperty(_GreekCulture, _DefaultIntFormat, _DefaultDecimalFormat, _DefaultDateFormat)) + separator);
                            }
                        }
                        //Αφαιρούμε το τελευταίο String διαχωρισμού
                        textBuilder.Length -= separator.Length;
                        //Τοποθετώ το τελικό κείμενο στο Clipboard
                        Clipboard.SetDataObject(textBuilder.ToString());
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        Clipboard.SetDataObject(string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Cursor.Current = Cursors.Default;
                ex.ShowException();
            }
        }

        private void _CopySelectionContent_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Έλεγχος αν υπάρχουν δεδομένα στο grid
                if (this.Rows.Count == 0)
                {
                    Clipboard.SetDataObject("");
                    //Έλεγχος αν έχουμε πολλαπλή επιλογή αλλά καμία επιλεγμένη γραμμή
                }
                else if (MultiSelect && SelectedRows.Count == 0)
                {
                    Clipboard.SetDataObject("");
                    //Έλεγχος αν έχουμε μοναδική επιλογή αλλά καμία επιλεγμένη γραμμή
                }
                else if ((!MultiSelect) && SelectedIndex == -1)
                {
                    Clipboard.SetDataObject("");
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                    //Ορίζουμε τους String Builder γραμμής και τελικού κειμένου
                    System.Text.StringBuilder textBuilder = new System.Text.StringBuilder();
                    System.Text.StringBuilder lineBuilder = new System.Text.StringBuilder();
                    //Παίρνουμε τα User Formats από το grid
                    String[] customColumnFormats = _CustomColumnFormatsArray;
                    //Φτιάχνουμε τη γραμμή με τα headers
                    foreach (DataGridViewColumn dtColumn in Columns)
                    {
                        //Μόνο αν είναι visible το Column
                        if (dtColumn.Visible)
                        {
                            //Αντικαθιστώ τους ειδικούς χαρακτήρες με κενά
                            lineBuilder.Append(RemoveSpecialChars(dtColumn.HeaderText) + "\t");
                        }
                    }
                    //Αφαιρούμε τον τελευταίο χαρακτήρα TAB
                    lineBuilder.Length -= 1;
                    //Προσθέτω τη γραμμή στο τελικό κείμενο
                    textBuilder.AppendLine(lineBuilder.ToString());
                    //Φτιάχνω τις γραμμές για τα δεδομένα του κάθε row των επιλεγμένων row
                    if ((MultiSelect))
                    {
                        foreach (DataGridViewRow dtRow in SelectedRows)
                        {
                            //Μηδενίζω τον StringBuilder της κάθε σειράς
                            lineBuilder.Length = 0;
                            //Για κάθε column
                            for (Int32 i = 0; i <= Columns.Count - 1; i++)
                            {
                                //Μόνο αν είναι visible το Column
                                if (Columns[i].Visible)
                                {
                                    object dtObj = dtRow.Cells[i].Value;
                                    if (i < customColumnFormats.Length)
                                    {
                                        lineBuilder.Append(RemoveSpecialChars(dtObj.GetClipboardTextFromProperty(_GreekCulture, customColumnFormats[i], customColumnFormats[i], customColumnFormats[i])) + "\t");
                                    }
                                    else
                                    {
                                        //Προσθέτουμε το κείμενο της επιλεγμένης στήλης στο τελικό κείμενο
                                        lineBuilder.Append(RemoveSpecialChars(dtObj.GetClipboardTextFromProperty(_GreekCulture, _DefaultIntFormat, _DefaultDecimalFormat, _DefaultDateFormat)) + "\t");
                                    }
                                }
                            }
                            //Αφαιρούμε το τελευταίο TAB
                            lineBuilder.Length -= 1;
                            //Προσθέτουμε τη γραμμή στο τελικό κείμενο
                            textBuilder.AppendLine(lineBuilder.ToString());
                        }
                    }
                    else
                    {
                        //Μηδενίζω τον StringBuilder της κάθε σειράς
                        lineBuilder.Length = 0;
                        //Για κάθε column
                        for (Int32 i = 0; i <= Columns.Count - 1; i++)
                        {
                            //Μόνο αν είναι visible το Column
                            if (Columns[i].Visible)
                            {
                                object dtObj = SelectedRows[0].Cells[i].Value;
                                //Προσθέτουμε το κείμενο της επιλεγμένης στήλης στο τελικό κείμενο
                                lineBuilder.Append(dtObj.GetClipboardTextFromProperty(_GreekCulture) + "\t");
                            }
                        }
                        //Αφαιρούμε το τελευταίο TAB
                        lineBuilder.Length -= 1;
                        //Προσθέτουμε τη γραμμή στο τελικό κείμενο
                        textBuilder.AppendLine(lineBuilder.ToString());
                    }
                    //Αφαιρούμε την τελευταία αλλαγή γραμμής (vbCrLf = CR LF 2 χαρακτήρες)
                    textBuilder.Length -= 2;
                    //Τοποθετώ το τελικό κείμενο στο Clipboard
                    Clipboard.SetDataObject(textBuilder.ToString());
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Cursor.Current = Cursors.Default;
                ex.ShowException();
            }
        }

        private void _ExportGridContents_Click(object sender, EventArgs e)
        {
            try
            {
                // Αν δεν υπάρχουν δεδομένα δεν κάνουμε τίποτα
                if(DataSource == null)
                {
                    return;
                }

                // Ζητάμε από τον χρήστη να επιλέξει αρχείο προς αποθήκευση
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Επιλέξτε αρχείο Excel για εξαγωγή...";
                sfd.Filter = "Αρχεία Excel (*.xlsx)|*.xlsx";
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = true;
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    if (DataSource is DataTable)
                    {
                        DataSet printDataSet = new DataSet();
                        printDataSet.Tables.Add((DataTable)DataSource);

                        gExcelHelper.DataSetToExcelFile(printDataSet, sfd.FileName);                        
                    }
                    else if (DataSource is IList)
                    {
                        gExcelHelper.ListToExcelFile((IList)DataSource, sfd.FileName);
                    }
                    else
                    {
                        throw new Exception(String.Format("Δεν υποστηρίζεται η εξαγωγή Excel για αυτόν τον τύπο δεδομένων ({0})!", DataSource.GetType().Name));
                    }
                    MessageBox.Show(String.Format("Το αρχείο {0} αποθηκεύτηκε με επιτυχία!", sfd.FileName), "Επιτυχία!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Cursor.Current = Cursors.Default;
                ex.ShowException();
            }
        }

        private void gDataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            try
            {
                e.ContextMenuStrip = _ContextMenu;
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private string RemoveSpecialChars(string str)
        {
            string strToReturn = str;
            strToReturn = strToReturn.Replace("\t", " ");
            strToReturn = strToReturn.Replace("\r\n", " ");
            strToReturn = strToReturn.Replace("\r", " ");
            strToReturn = strToReturn.Replace("\n", " ");
            strToReturn = strToReturn.Replace("\"", "");
            strToReturn = strToReturn.Replace("'", "");
            return strToReturn;
        }

        #endregion

        private void gDataGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                _LastClickedRowIndex = e.RowIndex;
                _LastClickedColumnIndex = e.ColumnIndex;

                if (Cursor.Current == Cursors.SizeWE)
                {
                    SetFooterRow(false, this.Width, this.Height, false);
                    _ColumnResizing = false;
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        protected Object _LastSelectedKey = null;

        protected Int32 _ColumnResizedIndex = -1;
        protected Boolean _ColumnResizing = false;

        private void gDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (_KeyColumnIndex > -1 && this.SelectedRows.Count > 0)
                {
                    _LastSelectedKey = this.SelectedRows[0].Cells[_KeyColumnIndex].Value;
                }
                else
                {
                    _LastSelectedKey = null;
                }

                if (Cursor.Current == Cursors.SizeWE && e.RowIndex == -1 && e.ColumnIndex > -1)
                {
                    _ColumnResizedIndex = e.ColumnIndex;

                    // Εάν είμαστε στα πρώτα 8 pixel, θεωρούμε ότι κάνουμε resize το αριστερό column
                    if (e.X < 8)
                    {
                        _ColumnResizedIndex = _ColumnResizedIndex - 1;
                    }

                    _ColumnResizing = true;
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void gDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && _ColumnResizing)
                {
                    var c = new DataGridViewColumnEventArgs(this.Columns[_ColumnResizedIndex]);

                    if(e.ColumnIndex > _ColumnResizedIndex)
                    {
                        // Αν έχουμε περάσει στο επόμενο column, τότε το e.X είναι μηδενισμένο ως προς το επόμενο column
                        // οπότε το προσθέτουμε
                        c.Column.Width += e.X;
                    }
                    else
                    {
                        // Αν είμαστε στο ίδιο column, τότε το e.X συμπίπτει με το Width
                        c.Column.Width = e.X;
                    }
                    SetFooterRow(false, this.Width, this.Height, false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            SetFooterRow(false, width, height, false);
            if (_ShowFooterRow)
            {
                if (NewGridHeight > -1)
                {
                    base.SetBoundsCore(x, y, width, NewGridHeight, specified);
                }
                else
                {
                    base.SetBoundsCore(x, y, width, height, specified);
                }
            }
            else
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        protected Boolean _InSetFooterRow = false;

        protected Int32 OriginalGridWidth = -1;
        protected Int32 OriginalGridHeight = -1;
        protected Int32 NewGridWidth = -1;
        protected Int32 NewGridHeight = -1;

        protected void SetFooterRow(Boolean ShowFooterChanged, Int32 width, Int32 height, Boolean calculateSums)
        {
            if (_InSetFooterRow || (this.IsInDesignMode() && !(Dock == DockStyle.None)))
            {
                return;
            }
            try
            {
                _InSetFooterRow = true;
                if (_FooterRow == null)
                {
                    _FooterRow = new gDataGridViewFooter(this);
                }
                if(this.Parent!= null)
                {
                    if(this.Parent.Controls.Find(_FooterRow.Name, true).Length == 0)
                    {
                        this.Parent.Controls.Add(_FooterRow);
                    }
                }

                // Ορίζω το ύψος του footer ίσο με το ύψος μιας σειράς του ColumnHeader
                Int32 footerRowHeight = this.ColumnHeadersDefaultCellStyle.Font.Height +
                            this.ColumnHeadersDefaultCellStyle.Padding.Top + this.ColumnHeadersDefaultCellStyle.Padding.Bottom +
                            8;

                Int32 footerRowWidth = -1;
                Int32 footerRowTop = -1;

                // Ελέγχω αν είναι η πρώτη φορά που υπολογίζονται οι διαστάσεις
                if (OriginalGridWidth == -1)
                {
                    // Αν είναι πρώτη φορά που υπολογίζονται οι διαστάσεις, τότε οι τιμές που μας έρχονται
                    // είναι σίγουρα οι τιμές όλου του Control
                    OriginalGridWidth = width;
                    OriginalGridHeight = height;

                    if (_ShowFooterRow)
                    {
                        // Αν εμφανίζεται το Footer, τότε υπολογίζω τις καινούριες τιμές των διαστάσεων
                        NewGridWidth = OriginalGridWidth;

                        NewGridHeight = OriginalGridHeight - footerRowHeight - 1;

                        footerRowWidth = OriginalGridWidth;

                        footerRowTop = this.Top + NewGridHeight + 1;
                    }
                    else
                    {
                        // Αν δεν εμφανίζεται το Footer, τότε οι καινούριες τιμές των διαστάσεων είναι ίδιες με αυτές που ήρθαν
                        NewGridWidth = OriginalGridWidth;

                        NewGridHeight = OriginalGridHeight;
                    }
                }
                else
                {
                    if (_ShowFooterRow)
                    {
                        if (ShowFooterChanged)
                        {
                            // Αν άλλαξε το ShowFooter σε true, άρα έρχεται ολόκληρο το control
                            // οπότε πρέπει να υπολογίσω τις τιμές του διαστάσεων
                            OriginalGridWidth = width;
                            OriginalGridHeight = height;

                            NewGridWidth = OriginalGridWidth;
                            NewGridHeight = OriginalGridHeight - footerRowHeight - 1;
                        }
                        else
                        {
                            // Αν δεν άλλαξε το ShowFooter, τότε ελέγχω αν το ύψος που είχε υπολογιστεί διαφέρει από το ύψος που έρχεται
                            // το οποίο ύψος ελέγχεται με το υπολογισμένο
                            if (NewGridHeight != height)
                            {
                                // άλλαξε το ύψος, άρα είναι ολόκληρο το control
                                OriginalGridWidth = width;
                                OriginalGridHeight = height;

                                NewGridWidth = OriginalGridWidth;
                                NewGridHeight = OriginalGridHeight - footerRowHeight - 1;
                            }
                            else
                            {
                                // Δεν άλλαξε το ύψος, άρα αλλάζω μόνο το πλάτος
                                OriginalGridWidth = width;

                                NewGridWidth = OriginalGridWidth;
                            }
                        }
                        footerRowWidth = OriginalGridWidth;
                        footerRowTop = this.Top + NewGridHeight;
                    }
                    else
                    {
                        if (ShowFooterChanged)
                        {
                            // Αν δεν άλλαξε το ShowFooter, τότε ελέγχω αν το ύψος που είχε υπολογιστεί διαφέρει από το ύψος που έρχεται
                            // το οποίο ύψος ελέγχεται με το αρχικό
                            if (OriginalGridHeight != height)
                            {
                                // άλλαξε το ύψος, άρα είναι ολόκληρο το control
                                OriginalGridWidth = width;
                                OriginalGridHeight = height;

                                NewGridWidth = OriginalGridWidth;
                                NewGridHeight = OriginalGridHeight - footerRowHeight - 1;
                            }
                            else
                            {
                                // Δεν άλλαξε το ύψος, άρα αλλάζω μόνο το πλάτος
                                OriginalGridWidth = width;

                                NewGridWidth = OriginalGridWidth;
                            }
                        }
                        else
                        {
                            // Αν άλλαξε το ShowFooter σε false, τότε επαναφέρω τις καινούριες διαστάσεις
                            OriginalGridWidth = width;
                            OriginalGridHeight = height;

                            NewGridWidth = width;
                            NewGridHeight = height;
                        }

                        footerRowWidth = OriginalGridWidth;
                        footerRowTop = -footerRowHeight;
                    }
                }

                if (_FooterRow.Visible != _ShowFooterRow)
                {
                    _FooterRow.Visible = _ShowFooterRow;
                }

                if (this.Size.Width != NewGridWidth || this.Size.Height != NewGridHeight)
                {
                    this.Size = new Size(NewGridWidth, NewGridHeight);
                }

                if (_FooterRow.Height != footerRowHeight)
                {
                    _FooterRow.Height = footerRowHeight;
                }

                if (_FooterRow.Size.Width != footerRowWidth || _FooterRow.Size.Height != footerRowHeight)
                {
                    _FooterRow.Size = new Size(footerRowWidth, footerRowHeight);
                }

                _FooterRow.UpdateSums(calculateSums);

                if (_FooterRow.Top != footerRowTop)
                {
                    _FooterRow.Top = footerRowTop;
                }

                if (_FooterRow.Left != this.Left)
                {
                    _FooterRow.Left = this.Left;
                }

                _InSetFooterRow = false;
            }
            catch (Exception)
            {
                _InSetFooterRow = false;
                throw;
            }
        }

        private void gDataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (_ShowFooterRow && _FooterRow != null)
                {
                    if(e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                    {
                        _FooterRow.UpdateSums(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public override void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
        {
            // Avoid running the OnSelectionEventChange while sorting
            _DataSourceSorting = true;
            base.Sort(dataGridViewColumn, direction);
            _DataSourceSorting = false;

            // After sorting, restore the previous selection
            if (_LastSelectedKey != null)
            {
                SetSelectedRowByKey(_LastSelectedKey);
            }
            else
            {
                ClearSelection();
            }
        }

        public override void Sort(IComparer comparer)
        {
            // Avoid running the OnSelectionEventChange while sorting
            _DataSourceSorting = true;
            base.Sort(comparer);
            _DataSourceSorting = false;

            // After sorting, restore the previous selection
            if (_LastSelectedKey != null)
            {
                SetSelectedRowByKey(_LastSelectedKey);
            }
            else
            {
                ClearSelection();
            }
        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            base.OnCurrentCellChanged(e);
        }

        protected bool _FromInsideSelectionChangedEvent = false;

        protected override void OnSelectionChanged(EventArgs e)
        {
            if (_FromInsideSelectionChangedEvent || _DataSourceSorting)
            {
                return;
            }

            // If DataSource is being set, avoid auto selecting the first row
            if (!AutoSelectFirstRowOnDataSourceChange && _DataSourcePopulating)
            {
                if (SelectedCells.Count > 0)
                {
                    _FromInsideSelectionChangedEvent = true;

                    // Remove the event handler, in order to avoid events in the containing form
                    string eventName = "DATAGRIDVIEWSELECTIONCHANGED";
                    object eventHandler = RemoveEventHandler(eventName);

                    // Clear the DataGridView selection
                    ClearSelection();
                    base.OnSelectionChanged(e);

                    // Add the form's event handler
                    AddEventHandler(eventName, eventHandler);

                    _FromInsideSelectionChangedEvent = false;
                }
            }
            else
            {
                base.OnSelectionChanged(e);
            }

            try
            {
                if (SelectedCells.Count == 0)
                {
                    _LastSelectedKey = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
