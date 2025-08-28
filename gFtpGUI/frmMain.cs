using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using gFtp;
using gpower2.gControls;
using gpower2.gSettings;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE1006 // Naming Styles
namespace gFtpGUI
{
    public partial class frmMain : Form
    {
        private static readonly Color DarkModeBackColor = Color.FromArgb(32, 32, 32);
        private static readonly Color DarkModeForeColor = Color.White;
        private static readonly Color DarkModeTextBoxBack = Color.FromArgb(64, 64, 64);
        private static readonly Color DarkModeButtonBack = Color.FromArgb(80, 80, 80);
        private static readonly Color LightModeBackColor = SystemColors.Control;
        private static readonly Color LightModeForeColor = SystemColors.ControlText;
        private static readonly Color LightModeTextBoxBack = SystemColors.Window;
        private static readonly Color LightModeButtonBack = SystemColors.Control;

        private readonly Settings _settings = new Settings();
        private readonly ToolTip _tooltip = new ToolTip();
        private readonly ILogger _logger;

        private gFTP _ftp = null;
        private frmQueue _FrmQueue = null;

        public frmMain()
        {
            InitializeComponent();
            // this.UseImmersiveDarkMode(true); // Removed as per requirement

            _logger = new TextBoxLogger(txtLog);
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            trvFtpFolders.ImageList = new ImageList();
            trvFtpFolders.ImageList.Images.Add("folder", Properties.Resources.Folder);
            trvFtpFolders.ImageList.Images.Add("folder_open", Properties.Resources.Folder_Open);
            trvFtpFolders.ImageKey = "folder";
            trvFtpFolders.SelectedImageKey = "folder_open";

            trvLocalFolders.ImageList = new ImageList();
            trvLocalFolders.ImageList.Images.Add("folder", Properties.Resources.Folder);
            trvLocalFolders.ImageList.Images.Add("folder_open", Properties.Resources.Folder_Open);
            trvLocalFolders.ImageKey = "folder";
            trvLocalFolders.SelectedImageKey = "folder_open";

            InitLocalDrives();
            InitFtpConnections();
            InitSettings();

            grdFtpFiles.MultiSelect = true;

            _tooltip.SetToolTip(btnLocalPathCreate, "Create a new local sub folder");
            _tooltip.SetToolTip(btnLocalPathDelete, "Delete the selected local folder");
            _tooltip.SetToolTip(btnLocalPathRename, "Rename the selected local folder");
            _tooltip.SetToolTip(btnLocalPathOpen, "Open the selected local folder in file explorer");

            _tooltip.SetToolTip(btnRefreshFtpPath, "Refresh the current FTP path");
            _tooltip.SetToolTip(btnRefreshLocalPath, "Refresh the current local path");

            this.chkDarkMode.CheckedChanged += new System.EventHandler(this.chkDarkMode_CheckedChanged);
            // It's important to set the initial state correctly.
            // UseImmersiveDarkMode must be called before ApplyTheme for certain visual elements.
            this.UseImmersiveDarkMode(this.chkDarkMode.Checked);
            ApplyTheme(this.chkDarkMode.Checked); // Apply initial theme based on checkbox state

            this.grdFtpFiles.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdFtpFiles_ColumnHeaderMouseClick);
            this.grdLocalFiles.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdLocalFiles_ColumnHeaderMouseClick);
        }

        private void grdFtpFiles_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Sorting is handled by the grid; re-apply theme after sort might repaint
            if (this.chkDarkMode.Checked) // Only if dark mode is active
            {
                // Optional: Could add a small delay if direct re-application has issues,
                // but try direct first. E.g., BeginInvoke might be too complex here.
                ApplyThemeToDataGridView(grdFtpFiles, true);
            }
            // No specific action needed for light mode as system handles it.
        }

        private void grdLocalFiles_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.chkDarkMode.Checked)
            {
                ApplyThemeToDataGridView(grdLocalFiles, true);
            }
        }

        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            this.UseImmersiveDarkMode(chkDarkMode.Checked);
            ApplyTheme(chkDarkMode.Checked);

            _settings.DarkMode = chkDarkMode.Checked;
            string settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.ini");
            gSettingsManager.SaveSettings(_settings, settingsPath);
        }

        private void ApplyThemeToDataGridView(gpower2.gControls.gDataGridView gdv, bool darkModeEnabled)
        {
            if (darkModeEnabled)
            {
                gdv.EnableHeadersVisualStyles = false;
                gdv.BackgroundColor = DarkModeBackColor;
                gdv.GridColor = Color.FromArgb(100, 100, 100);

                gdv.DefaultCellStyle.BackColor = DarkModeTextBoxBack;
                gdv.DefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 130);
                gdv.DefaultCellStyle.SelectionForeColor = Color.White;

                gdv.AlternatingRowsDefaultCellStyle.BackColor = DarkModeBackColor;
                gdv.AlternatingRowsDefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);
                gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

                gdv.ColumnHeadersDefaultCellStyle.BackColor = DarkModeButtonBack;
                gdv.ColumnHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkModeButtonBack;

                gdv.RowHeadersDefaultCellStyle.BackColor = DarkModeBackColor;
                gdv.RowHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                gdv.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);

                if (gdv.ContextMenuStrip != null)
                {
                    gdv.ContextMenuStrip.BackColor = DarkModeButtonBack;
                    gdv.ContextMenuStrip.ForeColor = DarkModeForeColor;
                    foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                    {
                        item.BackColor = DarkModeButtonBack;
                        item.ForeColor = DarkModeForeColor;
                        if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                        {
                            menuItem.DropDown.BackColor = DarkModeButtonBack;
                            menuItem.DropDown.ForeColor = DarkModeForeColor;
                            foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                            {
                                dropDownItem.BackColor = DarkModeButtonBack;
                                dropDownItem.ForeColor = DarkModeForeColor;
                            }
                        }
                    }
                }
            }
            else // Light Mode
            {
                gdv.EnableHeadersVisualStyles = true;
                gdv.BackgroundColor = System.Drawing.SystemColors.Window;
                gdv.GridColor = System.Drawing.Color.Gainsboro;
                gdv.RowHeadersVisible = false;
                gdv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

                // AlternatingRowsDefaultCellStyle (Matches dataGridViewCellStyle1 and dataGridViewCellStyle5)
                gdv.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
                gdv.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
                gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;

                // DefaultCellStyle (Matches dataGridViewCellStyle2 and dataGridViewCellStyle6)
                gdv.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
                gdv.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                gdv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                gdv.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                gdv.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
                gdv.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
                gdv.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;

                // ColumnHeadersDefaultCellStyle
                gdv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
                gdv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                gdv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
                gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Control;
                gdv.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.ControlText;

                // RowHeadersDefaultCellStyle
                gdv.RowHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
                gdv.RowHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                gdv.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
                gdv.RowHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
                gdv.RowHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;

                if (gdv.ContextMenuStrip != null)
                {
                    gdv.ContextMenuStrip.BackColor = System.Drawing.SystemColors.ControlLightLight;
                    gdv.ContextMenuStrip.ForeColor = System.Drawing.SystemColors.ControlText;
                    foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                    {
                        item.BackColor = System.Drawing.SystemColors.ControlLightLight;
                        item.ForeColor = System.Drawing.SystemColors.ControlText;
                        if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                        {
                            menuItem.DropDown.BackColor = System.Drawing.SystemColors.ControlLightLight;
                            menuItem.DropDown.ForeColor = System.Drawing.SystemColors.ControlText;
                            foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                            {
                                dropDownItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
                                dropDownItem.ForeColor = System.Drawing.SystemColors.ControlText;
                            }
                        }
                    }
                }
            }
            gdv.Refresh(); // Refresh the grid after applying styles
        }

        private void ApplyThemeToControl(Control c, bool darkModeEnabled)
        {
            if (darkModeEnabled)
            {
                c.BackColor = DarkModeBackColor;
                c.ForeColor = DarkModeForeColor;

                // Generic styling for most controls
                c.ForeColor = DarkModeForeColor;
                // General BackColor assignment, SplitContainer will be handled specifically
                c.BackColor = DarkModeBackColor;

                if (c is SplitContainer sc)
                {
                    sc.BackColor = DarkModeBackColor; // The splitter area itself
                    sc.Panel1.BackColor = DarkModeBackColor;
                    sc.Panel2.BackColor = DarkModeBackColor;
                    // The controls *within* Panel1 and Panel2 are handled by the recursive call
                }
                else if (c is TextBox || c is RichTextBox || c is ListBox)
                {
                    c.BackColor = DarkModeTextBoxBack;
                    if (c is TextBox tb) { tb.BorderStyle = BorderStyle.FixedSingle; }
                    else if (c is RichTextBox rtb) { rtb.BorderStyle = BorderStyle.FixedSingle; }
                    else if (c is ListBox lb) { lb.BorderStyle = BorderStyle.FixedSingle; }
                }
                else if (c is ComboBox cmb)
                {
                    ComboBoxStyle originalStyle = cmb.DropDownStyle;
                    cmb.DropDownStyle = ComboBoxStyle.Simple; // Temporarily change

                    cmb.FlatStyle = FlatStyle.Flat;
                    cmb.BackColor = DarkModeTextBoxBack;
                    cmb.ForeColor = DarkModeForeColor;
                    cmb.Invalidate(); // Force repaint for dark mode

                    cmb.DropDownStyle = originalStyle; // Restore

                    // Theme ContextMenuStrip for gComboBox instances
                    if (cmb is gpower2.gControls.gComboBox gcmb && gcmb.ContextMenuStrip != null)
                    {
                        gcmb.ContextMenuStrip.BackColor = DarkModeButtonBack;
                        gcmb.ContextMenuStrip.ForeColor = DarkModeForeColor;
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            item.BackColor = DarkModeButtonBack; // Ensure item back color
                            item.ForeColor = DarkModeForeColor;   // Ensure item fore color
                            if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                            {
                                menuItem.DropDown.BackColor = DarkModeButtonBack; // Ensure dropdown back color
                                menuItem.DropDown.ForeColor = DarkModeForeColor; // Ensure dropdown fore color
                                foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                                {
                                    dropDownItem.BackColor = DarkModeButtonBack; // Ensure item back color
                                    dropDownItem.ForeColor = DarkModeForeColor;   // Ensure item fore color
                                }
                            }
                        }
                    }
                }
                else if (c is Button btn)
                {
                    btn.BackColor = DarkModeButtonBack;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100); // Slightly lighter border
                    btn.FlatAppearance.BorderSize = 1;
                }
                else if (c is gpower2.gControls.gDataGridView gdv)
                {
                    gdv.EnableHeadersVisualStyles = false;
                    gdv.BackgroundColor = DarkModeBackColor;
                    gdv.GridColor = Color.FromArgb(100, 100, 100);

                    gdv.DefaultCellStyle.BackColor = DarkModeTextBoxBack;
                    gdv.DefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 130);
                    gdv.DefaultCellStyle.SelectionForeColor = Color.White;
                    // gdv.DefaultCellStyle.Font = new Font("Segoe UI", 9F); 

                    gdv.AlternatingRowsDefaultCellStyle.BackColor = DarkModeBackColor;
                    gdv.AlternatingRowsDefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);
                    gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

                    gdv.ColumnHeadersDefaultCellStyle.BackColor = DarkModeButtonBack;
                    gdv.ColumnHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkModeButtonBack;
                    // gdv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                    gdv.RowHeadersDefaultCellStyle.BackColor = DarkModeBackColor;
                    gdv.RowHeadersDefaultCellStyle.ForeColor = DarkModeForeColor;
                    gdv.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 150);
                    // gdv.RowHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F);

                    if (gdv.ContextMenuStrip != null)
                    {
                        gdv.ContextMenuStrip.BackColor = DarkModeButtonBack;
                        gdv.ContextMenuStrip.ForeColor = DarkModeForeColor;
                        foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                        {
                            item.BackColor = DarkModeButtonBack;
                            item.ForeColor = DarkModeForeColor;
                            if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                            {
                                menuItem.DropDown.BackColor = DarkModeButtonBack;
                                menuItem.DropDown.ForeColor = DarkModeForeColor;
                                foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                                {
                                    dropDownItem.BackColor = DarkModeButtonBack;
                                    dropDownItem.ForeColor = DarkModeForeColor;
                                }
                            }
                        }
                    }
                }
                else if (c is TreeView tv)
                {
                    tv.BackColor = DarkModeTextBoxBack;
                    tv.LineColor = Color.FromArgb(100, 100, 100);
                }
                else if (c is GroupBox gb)
                {
                    gb.ForeColor = DarkModeForeColor;
                }
                else if (c is Label lbl)
                {
                    if (lbl.Parent is GroupBox || lbl.Parent is Panel || lbl.Parent is SplitContainer)
                    {
                        lbl.BackColor = Color.Transparent;
                    }
                    else
                    {
                        lbl.BackColor = DarkModeBackColor;
                    }
                }
                else if (c is Panel panel)
                {
                    panel.BackColor = DarkModeBackColor;
                }
                else if (c is CheckBox chk)
                {
                    chk.ForeColor = DarkModeForeColor;
                    if (chk.Parent is GroupBox || chk.Parent is Panel)
                    {
                        chk.BackColor = Color.Transparent;
                    }
                    else
                    {
                        chk.BackColor = DarkModeBackColor;
                    }
                }
            }
            else // Light Mode
            {
                c.ForeColor = LightModeForeColor;
                c.BackColor = LightModeBackColor;

                if (c is SplitContainer sc)
                {
                    sc.BackColor = SystemColors.Control;
                    sc.Panel1.BackColor = SystemColors.Control;
                    sc.Panel2.BackColor = SystemColors.Control;
                }
                else if (c is TextBox || c is RichTextBox || c is ListBox)
                {
                    c.BackColor = LightModeTextBoxBack;
                    if (c is TextBox tb) { tb.BorderStyle = BorderStyle.Fixed3D; }
                    else if (c is RichTextBox rtb) { rtb.BorderStyle = BorderStyle.Fixed3D; }
                    else if (c is ListBox lb) { lb.BorderStyle = BorderStyle.FixedSingle; }
                }
                else if (c is ComboBox cmb)
                {
                    ComboBoxStyle originalStyle = cmb.DropDownStyle;
                    cmb.DropDownStyle = ComboBoxStyle.Simple; // Temporarily change

                    cmb.FlatStyle = FlatStyle.Standard;
                    cmb.BackColor = SystemColors.Window;
                    cmb.ForeColor = SystemColors.ControlText;
                    cmb.Invalidate(); // Force repaint for light mode

                    cmb.DropDownStyle = originalStyle; // Restore

                    if (cmb is gpower2.gControls.gComboBox gcmb && gcmb.ContextMenuStrip != null)
                    {
                        gcmb.ContextMenuStrip.BackColor = SystemColors.ControlLightLight;
                        gcmb.ContextMenuStrip.ForeColor = SystemColors.ControlText;
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            item.BackColor = SystemColors.ControlLightLight; // Ensure item back color is light
                            item.ForeColor = SystemColors.ControlText;   // Ensure item fore color is light
                            if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                            {
                                menuItem.DropDown.BackColor = SystemColors.ControlLightLight;
                                menuItem.DropDown.ForeColor = SystemColors.ControlText;
                                foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                                {
                                    dropDownItem.BackColor = SystemColors.ControlLightLight;
                                    dropDownItem.ForeColor = SystemColors.ControlText;
                                }
                            }
                        }
                    }
                }
                else if (c is Button btn)
                {
                    btn.BackColor = LightModeButtonBack;
                    btn.FlatStyle = FlatStyle.System;
                    btn.FlatAppearance.BorderColor = SystemColors.ControlDark;
                    btn.FlatAppearance.BorderSize = 1;
                }
                else if (c is gpower2.gControls.gDataGridView gdv)
                {
                    gdv.EnableHeadersVisualStyles = true;
                    gdv.BackgroundColor = SystemColors.ControlDarkDark; // Corrected from AppWorkspace
                    gdv.GridColor = SystemColors.ControlDark;

                    gdv.DefaultCellStyle.BackColor = SystemColors.Window;
                    gdv.DefaultCellStyle.ForeColor = SystemColors.ControlText;
                    gdv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    gdv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    gdv.DefaultCellStyle.Font = SystemFonts.DefaultFont;

                    gdv.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Window; // Corrected
                    gdv.AlternatingRowsDefaultCellStyle.ForeColor = SystemColors.ControlText;
                    gdv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    gdv.AlternatingRowsDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    // gdv.AlternatingRowsDefaultCellStyle.Font = SystemFonts.DefaultFont;

                    gdv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                    gdv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText; // Corrected
                    gdv.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
                    gdv.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
                    gdv.ColumnHeadersDefaultCellStyle.Font = SystemFonts.DefaultFont;

                    gdv.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                    gdv.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
                    gdv.RowHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    gdv.RowHeadersDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    gdv.RowHeadersDefaultCellStyle.Font = SystemFonts.DefaultFont;

                    if (gdv.ContextMenuStrip != null)
                    {
                        gdv.ContextMenuStrip.BackColor = SystemColors.ControlLightLight;
                        gdv.ContextMenuStrip.ForeColor = SystemColors.ControlText;
                        foreach (ToolStripItem item in gdv.ContextMenuStrip.Items)
                        {
                            item.BackColor = SystemColors.ControlLightLight;
                            item.ForeColor = SystemColors.ControlText;
                            if (item is ToolStripMenuItem menuItem && menuItem.DropDown != null)
                            {
                                menuItem.DropDown.BackColor = SystemColors.ControlLightLight;
                                menuItem.DropDown.ForeColor = SystemColors.ControlText;
                                foreach (ToolStripItem dropDownItem in menuItem.DropDown.Items)
                                {
                                    dropDownItem.BackColor = SystemColors.ControlLightLight;
                                    dropDownItem.ForeColor = SystemColors.ControlText;
                                }
                            }
                        }
                    }
                }
                else if (c is TreeView tv)
                {
                    tv.BackColor = LightModeTextBoxBack;
                    tv.LineColor = SystemColors.GrayText;
                    tv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                }
                else if (c is GroupBox gb)
                {
                    // gb.ForeColor is already LightModeForeColor
                }
                else if (c is Label lbl)
                {
                    if (lbl.Parent is GroupBox || lbl.Parent is Panel || lbl.Parent is SplitContainer)
                    {
                        lbl.BackColor = Color.Transparent;
                    }
                    else
                    {
                        lbl.BackColor = LightModeBackColor;
                    }
                }
                else if (c is Panel panel)
                {
                    panel.BackColor = LightModeBackColor;
                }
                else if (c is CheckBox chk)
                {
                    if (chk.Parent is GroupBox || chk.Parent is Panel)
                    {
                        chk.BackColor = Color.Transparent;
                    }
                    else
                    {
                        chk.BackColor = LightModeBackColor;
                    }
                }
            }

            if (c.Name == "txtFtpPath" || c.Name == "txtLocalPath" || c.Name == "txtLocalDriveInfo" || c.Name == "txtLog" || c.Name == "txtAria")
            {
                c.BackColor = darkModeEnabled ? DarkModeTextBoxBack : LightModeTextBoxBack;
                c.ForeColor = darkModeEnabled ? DarkModeForeColor : LightModeForeColor;
            }

            if (c.Controls.Count > 0)
            {
                ApplyThemeToControlCollection(c.Controls, darkModeEnabled);
            }
        }

        private void ApplyThemeToControlCollection(Control.ControlCollection controls, bool darkModeEnabled)
        {
            foreach (Control c in controls)
            {
                ApplyThemeToControl(c, darkModeEnabled);
            }
        }

        private void ApplyTheme(bool darkModeEnabled)
        {
            ApplyThemeToControlCollection(this.Controls, darkModeEnabled);
            this.BackColor = darkModeEnabled ? DarkModeBackColor : LightModeBackColor;

            // For controls not directly on this.Controls (e.g. in SplitContainer Panels)
            // We need to ensure panels within split containers are also themed.
            // This can be repetitive if ApplyThemeToControlCollection is thorough,
            // but explicit calls can ensure critical areas are covered.
            // Example for spltFiles panels:
            if (spltFiles != null)
            {
                ApplyThemeToControlCollection(spltFiles.Panel1.Controls, darkModeEnabled);
                ApplyThemeToControlCollection(spltFiles.Panel2.Controls, darkModeEnabled);
            }
            if (spltFtp != null)
            {
                ApplyThemeToControlCollection(spltFtp.Panel1.Controls, darkModeEnabled);
                ApplyThemeToControlCollection(spltFtp.Panel2.Controls, darkModeEnabled);
            }
            if (spltLocal != null)
            {
                ApplyThemeToControlCollection(spltLocal.Panel1.Controls, darkModeEnabled);
                ApplyThemeToControlCollection(spltLocal.Panel2.Controls, darkModeEnabled);
            }
            // Ensure top-level group boxes are themed (their children are handled by recursion)
            ApplyThemeToControlCollection(grpFtpServer.Controls, darkModeEnabled);
            ApplyThemeToControlCollection(grpActions.Controls, darkModeEnabled);
            ApplyThemeToControlCollection(grpLog.Controls, darkModeEnabled);

            // Force refresh of DataGridViews as their appearance can be tricky
            // grdFtpFiles.Refresh(); // Replaced by direct ApplyThemeToDataGridView call
            // grdLocalFiles.Refresh(); // Replaced by direct ApplyThemeToDataGridView call
            ApplyThemeToDataGridView(this.grdFtpFiles, darkModeEnabled);
            ApplyThemeToDataGridView(this.grdLocalFiles, darkModeEnabled);

            if (_FrmQueue != null && !_FrmQueue.IsDisposed)
            {
                _FrmQueue.ApplyTheme(darkModeEnabled);
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            _FrmQueue = new frmQueue();
            _FrmQueue.Show();
            // Apply theme to frmQueue as well if it's part of the same visual style
            if (_FrmQueue != null && !_FrmQueue.IsDisposed) // Ensure it's created and not disposed
            {
                _FrmQueue.ApplyTheme(this.chkDarkMode.Checked);
            }
            // Ensure DataGridViews are themed correctly after form is shown
            ApplyThemeToDataGridView(this.grdFtpFiles, this.chkDarkMode.Checked);
            ApplyThemeToDataGridView(this.grdLocalFiles, this.chkDarkMode.Checked);
        }

        private void InitLocalDrives()
        {
            DriveInfo[] localDrives = DriveInfo.GetDrives();
            cmbLocalDrives.DisplayMember = "Name";
            cmbLocalDrives.ValueMember = "Name";
            cmbLocalDrives.DataSource = localDrives;
        }

        private void InitFtpConnections()
        {
            String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connections.xml");
            if (!File.Exists(filename))
            {
                cmbFtpConnections.DisplayMember = "Server";
                cmbFtpConnections.DataSource = new List<FtpConnection>();
            }
            else
            {
                cmbFtpConnections.DisplayMember = "Server";

                XmlSerializer serializer = new XmlSerializer(typeof(List<FtpConnection>));
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    cmbFtpConnections.DataSource = (List<FtpConnection>)serializer.Deserialize(XmlReader.Create(fs));
                }
            }
        }

        private void InitSettings()
        {
            String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.ini");

            gSettingsManager.ReadSettings(_settings, filename);
            if (!String.IsNullOrWhiteSpace(_settings.Aria2Path))
            {
                txtAria.Text = _settings.Aria2Path;
            }
            else
            {
                txtAria.Clear();
            }
            chkDarkMode.Checked = _settings.DarkMode;
            // The theme is already applied in the constructor based on chkDarkMode.Checked,
            // which is now updated from settings. If InitSettings were called *after*
            // initial theme application, we'd need to re-apply here:
            // ApplyTheme(_settings.DarkMode);
            // UseImmersiveDarkMode(_settings.DarkMode);
        }

        private void ClearFtpFolder()
        {
            trvFtpFolders.Nodes.Clear();
            grdFtpFiles.DataSource = null;
            txtFtpPath.Clear();
        }

        private void ClearLocalFolder()
        {
            trvLocalFolders.Nodes.Clear();
            grdLocalFiles.DataSource = null;
            txtLocalPath.Clear();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFtpFolder();

                grpFtpServer.Enabled = false;
                grpFtpServer.Cursor = Cursors.WaitCursor;

                grpFtpFiles.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;

                _ftp = new gFTP(txtFtpServer.Text, txtUsername.Text, txtPassword.Text, _logger);
                FtpFolder f = await _ftp.GetFtpFolderDetailsAsync("", 0);
                FillFtpDirectoryTree(f);

                grpFtpServer.Enabled = true;
                grpFtpServer.Cursor = Cursors.Default;

                grpFtpFiles.Enabled = true;
                grpFtpFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpFtpServer.Cursor = Cursors.Default;
                grpFtpFiles.Cursor = Cursors.Default;

                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpServer.Enabled = true;
                grpFtpFiles.Enabled = true;
            }
        }

        private void FillFtpDirectoryTree(FtpFolder argFtpInfo)
        {
            TreeNode rootNode = new TreeNode(argFtpInfo.Name)
            {
                Tag = argFtpInfo,
                ImageKey = "folder",
                SelectedImageKey = "folder_open"
            };
            GetFtpDirectories(rootNode, 0);
            // First clear nodes
            trvFtpFolders.Nodes.Clear();
            // Add the new nodes
            trvFtpFolders.Nodes.Add(rootNode);
        }

        private void GetFtpDirectories(TreeNode nodeToAddTo, Int32 argLevel)
        {
            if (argLevel >= 0)
            {
                // First clear the nodes
                nodeToAddTo.Nodes.Clear();
                IList<FtpFolder> subDirs = null;
                try
                {
                    subDirs = (nodeToAddTo.Tag as FtpFolder).Folders;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (subDirs != null)
                {
                    foreach (FtpFolder subDir in subDirs)
                    {
                        //FtpFolder f = ftp.GetFtpFolderDetails(subDir.FullPath, 0);
                        TreeNode aNode = new TreeNode(subDir.Name, 0, 0)
                        {
                            Tag = subDir,
                            ImageKey = "folder",
                            SelectedImageKey = "folder_open"
                        };
                        GetFtpDirectories(aNode, argLevel - 1);
                        nodeToAddTo.Nodes.Add(aNode);
                    }
                }
            }
        }

        private async void trvFtpFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                grpFtpFiles.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;

                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = e.Node;
                selNode.Tag = await _ftp.GetFtpFolderDetailsAsync((selNode.Tag as FtpFolder).FullPath, 0);
                FtpFolder selDirInfo = selNode.Tag as FtpFolder;
                txtFtpPath.Text = selDirInfo.FullPath;
                GetFtpDirectories(selNode, 0);

                // Read the contents and add them to the ListView
                // First clear the list view contents
                grdFtpFiles.DataSource = null;
                ApplyThemeToControl(grdFtpFiles, chkDarkMode.Checked); 
                grdFtpFiles.Refresh();

                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem
                    {
                        Type = "Directory",
                        Name = "...",
                        Icon = Properties.Resources.folder1.ToBitmap(),
                        Size = null
                    };
                    items.Add(itemUp);
                }

                IList<FtpFolder> localDirs = null;
                try
                {
                    localDirs = selDirInfo.Folders;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (FtpFolder dir in localDirs)
                    {
                        FileItem item = new FileItem
                        {
                            Type = "Directory",
                            Name = dir.Name,
                            Icon = Properties.Resources.folder1.ToBitmap(),
                            Date = dir.Date,
                            Size = null
                        };
                        items.Add(item);
                    }
                }

                IList<FtpFile> localFiles = null;
                try
                {
                    localFiles = selDirInfo.Files;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FtpFile file in localFiles)
                    {
                        FileItem item = new FileItem
                        {
                            Type = file.Extension(),
                            Name = file.Name,
                            Icon = IconReader.GetFileIcon(file.Extension(), IconReader.IconSize.Small, false).ToBitmap(),
                            Date = file.Date,
                            Size = new gFtpGUI.FileSize(file.Size)
                        };
                        items.Add(item);
                    }
                }

                grdFtpFiles.DataSource = items;
                ApplyThemeToDataGridView(grdFtpFiles, chkDarkMode.Checked);

                grpFtpFiles.Enabled = true;

                grpFtpFiles.PerformLayout();

                grpFtpFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpFtpFiles.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpFiles.Enabled = true;
            }
        }

        private async void cmbLocalDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearLocalFolder();
                if (cmbLocalDrives.SelectedIndex > -1)
                {
                    DriveInfo selDrive = cmbLocalDrives.SelectedItem as DriveInfo;
                    if (selDrive.IsReady)
                    {
                        txtLocalDriveInfo.Text = String.Format("[{0}] | Size {1} | Free space {2}", selDrive.VolumeLabel, SizeHelper.GetSize(selDrive.TotalSize), SizeHelper.GetSize(selDrive.TotalFreeSpace));
                        txtLocalPath.Text = selDrive.RootDirectory.FullName;
                        await FillLocalDirectoryTreeAsync(selDrive.RootDirectory);
                    }
                    else
                    {
                        txtLocalDriveInfo.Text = "Device not ready";
                    }
                }
                else
                {
                    txtLocalDriveInfo.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefreshLocalPath_Click(object sender, EventArgs e)
        {
            try
            {
                // Refresh the local drive info
                if (cmbLocalDrives.SelectedIndex > -1)
                {
                    DriveInfo selDrive = cmbLocalDrives.SelectedItem as DriveInfo;
                    if (selDrive.IsReady)
                    {
                        txtLocalDriveInfo.Text = String.Format("[{0}] | Size {1} | Free space {2}", selDrive.VolumeLabel, SizeHelper.GetSize(selDrive.TotalSize), SizeHelper.GetSize(selDrive.TotalFreeSpace));
                    }
                    else
                    {
                        txtLocalDriveInfo.Text = "Device not ready";
                    }
                }

                if (trvLocalFolders.SelectedNode == null)
                {
                    return;
                }

                grpLocalFiles.Enabled = false;
                grpLocalFiles.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = trvLocalFolders.SelectedNode;
                DirectoryInfo selDirInfo = selNode.Tag as DirectoryInfo;
                txtLocalPath.Text = selDirInfo.FullName;
                await GetDirectoriesAsync(selNode, 1);

                // Read the contents and add them to the ListView
                // Keep the sorted column and order
                DataGridViewColumn sortColumn = grdLocalFiles.SortedColumn;
                SortOrder sortOrder = grdLocalFiles.SortOrder;
                int oldSelection = grdLocalFiles.SelectedIndex;
                int oldVerticalOffset = grdLocalFiles.FirstDisplayedScrollingRowIndex;
                // First clear the list view contents
                grdLocalFiles.DataSource = null;
                // ApplyThemeToControl(grdLocalFiles, chkDarkMode.Checked); grdLocalFiles.Refresh(); // This call is removed
                ApplyThemeToDataGridView(grdLocalFiles, chkDarkMode.Checked);


                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem
                    {
                        Type = "Directory",
                        Name = "...",
                        Icon = Properties.Resources.folder1.ToBitmap(),
                        Size = null
                    };
                    items.Add(itemUp);
                }

                DirectoryInfo[] localDirs = null;
                try
                {
                    localDirs = selDirInfo.GetDirectories();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (DirectoryInfo dir in localDirs)
                    {
                        FileItem item = new FileItem
                        {
                            Type = "Directory",
                            Name = dir.Name,
                            Icon = Properties.Resources.folder1.ToBitmap(),
                            Date = dir.LastWriteTime,
                            Size = null
                        };
                        items.Add(item);
                    }
                }

                FileInfo[] localFiles = null;
                try
                {
                    localFiles = selDirInfo.GetFiles();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FileInfo file in localFiles)
                    {
                        FileItem item = new FileItem
                        {
                            Type = file.Extension,
                            Name = file.Name,
                            Icon = IconReader.GetFileIcon(file.Extension, IconReader.IconSize.Small, false).ToBitmap(),
                            Date = file.LastWriteTime,
                            Size = new gFtpGUI.FileSize(file.Length)
                        };
                        items.Add(item);
                    }
                }

                grdLocalFiles.DataSource = items;
                ApplyThemeToDataGridView(grdLocalFiles, chkDarkMode.Checked);

                // Set the sortedColumn and order again
                if (sortColumn != null)
                {
                    // Find the new DataGridViewColumn
                    DataGridViewColumn newSortColumn = grdLocalFiles.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == sortColumn.Name);
                    if (newSortColumn != null)
                    {
                        if (sortOrder == SortOrder.Ascending)
                        {
                            grdLocalFiles.Sort(newSortColumn, ListSortDirection.Ascending);
                        }
                        else if (sortOrder == SortOrder.Descending)
                        {
                            grdLocalFiles.Sort(newSortColumn, ListSortDirection.Descending);
                        }
                        ApplyThemeToDataGridView(grdLocalFiles, chkDarkMode.Checked);
                    }
                }

                if (oldVerticalOffset > grdLocalFiles.Rows.Count - 1)
                {
                    oldVerticalOffset = grdLocalFiles.Rows.Count - 1;
                }
                grdLocalFiles.FirstDisplayedScrollingRowIndex = oldVerticalOffset;

                if (oldSelection > grdLocalFiles.Rows.Count - 1)
                {
                    oldSelection = grdLocalFiles.Rows.Count - 1;
                }
                grdLocalFiles.SetSelectedRowByIndex(oldSelection, false);

                grdLocalFiles.PerformLayout();

                grpLocalFiles.Enabled = true;
                grpLocalFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpLocalFiles.Cursor = Cursors.Default;

                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpLocalFiles.Enabled = true;
            }
        }

        private async Task FillLocalDirectoryTreeAsync(DirectoryInfo argInfo)
        {
            TreeNode rootNode = new TreeNode(argInfo.Name)
            {
                Tag = argInfo,
                ImageKey = "folder",
                SelectedImageKey = "folder_open"
            };

            await GetDirectoriesAsync(rootNode, 1);

            // First clear nodes
            trvLocalFolders.Nodes.Clear();

            // Add the new nodes
            trvLocalFolders.Nodes.Add(rootNode);
        }

        private async Task GetDirectoriesAsync(TreeNode nodeToAddTo, Int32 argLevel)
        {
            if (argLevel > 0)
            {
                // First clear the nodes
                nodeToAddTo.Nodes.Clear();
                DirectoryInfo[] subDirs = null;
                try
                {
                    subDirs = await (nodeToAddTo.Tag as DirectoryInfo).GetDirectoriesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (subDirs != null)
                {
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        TreeNode aNode = new TreeNode(subDir.Name, 0, 0)
                        {
                            Tag = subDir,
                            ImageKey = "folder",
                            SelectedImageKey = "folder_open"
                        };
                        await GetDirectoriesAsync(aNode, argLevel - 1);
                        nodeToAddTo.Nodes.Add(aNode);
                    }
                }
            }
        }

        private async void trvLocalFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                grpLocalFiles.Enabled = false;
                grpLocalFiles.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = e.Node;
                DirectoryInfo selDirInfo = selNode.Tag as DirectoryInfo;
                txtLocalPath.Text = selDirInfo.FullName;
                await GetDirectoriesAsync(selNode, 1);

                // Read the contents and add them to the ListView
                // First clear the list view contents
                grdLocalFiles.DataSource = null;

                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem
                    {
                        Type = "Directory",
                        Name = "...",
                        Icon = Properties.Resources.folder1.ToBitmap(),
                        Size = null
                    };
                    items.Add(itemUp);
                }

                DirectoryInfo[] localDirs = null;
                try
                {
                    localDirs = selDirInfo.GetDirectories();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (DirectoryInfo dir in localDirs)
                    {
                        FileItem item = new FileItem
                        {
                            Type = "Directory",
                            Name = dir.Name,
                            Icon = Properties.Resources.folder1.ToBitmap(),
                            Date = dir.LastWriteTime,
                            Size = null
                        };
                        items.Add(item);
                    }
                }

                FileInfo[] localFiles = null;
                try
                {
                    localFiles = selDirInfo.GetFiles();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FileInfo file in localFiles)
                    {
                        FileItem item = new FileItem
                        {
                            Type = file.Extension,
                            Name = file.Name,
                            Icon = IconReader.GetFileIcon(file.Extension, IconReader.IconSize.Small, false).ToBitmap(),
                            Date = file.LastWriteTime,
                            Size = new gFtpGUI.FileSize(file.Length)
                        };
                        items.Add(item);
                    }
                }

                grdLocalFiles.DataSource = items;
                ApplyThemeToDataGridView(grdLocalFiles, chkDarkMode.Checked);
                // ApplyThemeToDataGridView already calls Refresh, so no need for ApplyThemeToControl or grdLocalFiles.Refresh()

                grpLocalFiles.Enabled = true;
                grpLocalFiles.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpLocalFiles.Cursor = Cursors.Default;

                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpLocalFiles.Enabled = true;
            }
        }

        private void txtAria_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // check if the drop data is actually a file or folder
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    String[] s = (String[])e.Data.GetData(DataFormats.FileDrop, false);
                    ((TextBox)sender).Text = s[0];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAria_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.All;
                }
                else
                    e.Effect = DragDropEffects.None;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseAria_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Select aria2...",
                    FileName = "aria2c.exe"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtAria.Text = ofd.FileName;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDeleteRemoteFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdFtpFiles.SelectedIndex == -1)
                {
                    throw new Exception("No Ftp File selected!");
                }

                // Ask for confirmation from user
                if (MessageBox.Show(
                    "Are you sure you want to delete remote files?",
                    "Are you sure?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                grpFtpFiles.Enabled = false;
                grpFtpServer.Enabled = false;
                btnDeleteRemoteFile.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;
                grpFtpServer.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                foreach (FileItem f in grdFtpFiles.SelectedItems)
                {
                    if (f.Type != "Directory")
                    {
                        await _ftp.DeleteRemoteFileAsync(UrlHelper.Combine(txtFtpPath.Text, f.Name));
                    }
                    else
                    {
                        // First we have to get all the Folders and Directories are inside
                        FtpFolder fd = await _ftp.GetFtpFolderDetailsAsync(UrlHelper.Combine(txtFtpPath.Text, f.Name), Int32.MaxValue);
                        await DeleteFtpFolderAsync(fd);
                    }
                }

                // Refresh the ftp grid
                TaskCompletionSource<int> t = new TaskCompletionSource<int>();
                btnRefreshFtpPath_Click(t, null);

                await t.Task;

                grpFtpFiles.Cursor = Cursors.Default;
                grpFtpServer.Cursor = Cursors.Default;

                MessageBox.Show($"The remote file(s) was deleted successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                grpFtpFiles.Enabled = true;
                grpFtpServer.Enabled = true;
                btnDeleteRemoteFile.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpFtpFiles.Cursor = Cursors.Default;
                grpFtpServer.Cursor = Cursors.Default;

                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpFiles.Enabled = true;
                grpFtpServer.Enabled = true;
                btnDeleteRemoteFile.Enabled = true;
            }
        }

        private async Task DeleteFtpFolderAsync(FtpFolder argFolder)
        {
            // Delete all the files in the folder
            foreach (FtpFile file in argFolder.Files)
            {
                await _ftp.DeleteRemoteFileAsync(UrlHelper.Combine(argFolder.FullPath, file.Name));
            }

            // Delete all the sub folders
            foreach (FtpFolder folder in argFolder.Folders)
            {
                if (folder.Name != "..." && folder.Name != ".." && folder.Name != ".")
                {
                    await DeleteFtpFolderAsync(folder);
                }
            }

            // Delete the folder
            await _ftp.DeleteRemoteFolderAsync(argFolder.FullPath);
        }


        private async void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdFtpFiles.SelectedIndex == -1)
                {
                    throw new Exception("No Ftp File selected!");
                }
                if (String.IsNullOrWhiteSpace(txtLocalPath.Text))
                {
                    throw new Exception("No local path was selected!");
                }
                if (String.IsNullOrWhiteSpace(txtAria.Text))
                {
                    throw new Exception("No Aria2 path was provided!");
                }

                grpFtpFiles.Enabled = false;
                grpFtpServer.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;
                grpFtpServer.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                var selectedFtpItems = grdFtpFiles.SelectedItems.Cast<FileItem>().OrderBy(f => f.Name);

                foreach (FileItem f in selectedFtpItems)
                {
                    if (f.Type != "Directory")
                    {
                        Job j = new gFtpGUI.Job
                        {
                            AriaPath = txtAria.Text,
                            FtpPath = txtFtpPath.Text,
                            FtpFilename = f.Name,
                            LocalPath = txtLocalPath.Text,
                            LocalFilename = f.Name,
                            Size = f.Size,
                            Ftp = _ftp
                        };

                        await _FrmQueue.AddJobAsync(j);
                    }
                    else
                    {
                        // First we have to get all the Folders and Directories are inside
                        FtpFolder fd = await _ftp.GetFtpFolderDetailsAsync(UrlHelper.Combine(txtFtpPath.Text, f.Name), Int32.MaxValue);
                        await DownloadFtpFolderAsync(fd, txtLocalPath.Text);
                    }
                }

                grpFtpFiles.Enabled = true;
                grpFtpServer.Enabled = true;
                grpFtpFiles.Cursor = Cursors.Default;
                grpFtpServer.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                grpFtpFiles.Cursor = Cursors.Default;
                grpFtpServer.Cursor = Cursors.Default;

                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpFiles.Enabled = true;
                grpFtpServer.Enabled = true;
            }
        }

        private async Task DownloadFtpFolderAsync(FtpFolder argFolder, String argLocalRoot)
        {
            // Make all the necessary local folders
            String localDestFolder = Path.Combine(argLocalRoot, argFolder.Name);
            if (!await DirectoryAsync.ExistsAsync(localDestFolder))
            {
                await DirectoryAsync.CreateDirectoryAsync(localDestFolder);
            }
            List<Job> jobs = new List<gFtpGUI.Job>();
            foreach (FtpFile file in argFolder.Files.OrderBy(f => f.Name))
            {
                Job j = new gFtpGUI.Job
                {
                    AriaPath = txtAria.Text,
                    FtpPath = argFolder.FullPath,
                    FtpFilename = file.Name,
                    LocalPath = localDestFolder,
                    LocalFilename = file.Name,
                    Size = new FileSize(file.Size),
                    Ftp = _ftp
                };

                jobs.Add(j);

                Application.DoEvents();
            }
            await _FrmQueue.AddJobsAsync(jobs);
            foreach (FtpFolder folder in argFolder.Folders)
            {
                if (folder.Name != "..." && folder.Name != ".." && folder.Name != ".")
                {
                    await DownloadFtpFolderAsync(folder, localDestFolder);
                }
            }
        }

        private void grdFtpFiles_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (grdFtpFiles.SelectedIndex == -1)
                {
                    return;
                }
                if (grdFtpFiles.LastClickedRowIndex == -1)
                {
                    return;
                }
                FileItem f = grdFtpFiles.SelectedItem as FileItem;
                if (f.Type == "Directory")
                {
                    if (f.Name == "..." || f.Name == ".." || f.Name == ".")
                    {
                        if (trvFtpFolders.SelectedNode.Parent != null)
                        {
                            trvFtpFolders.SelectedNode = trvFtpFolders.SelectedNode.Parent;
                        }
                    }
                    else
                    {
                        String argPath = UrlHelper.Combine(txtFtpPath.Text, f.Name).Replace("//", "/");
                        TreeNode node = FindFtpNode(trvFtpFolders.Nodes[0], argPath);
                        if (node != null)
                        {
                            trvFtpFolders.SelectedNode = node;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void grdLocalFiles_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (grdLocalFiles.SelectedIndex == -1)
                {
                    return;
                }
                if (grdLocalFiles.LastClickedRowIndex == -1)
                {
                    return;
                }
                FileItem f = grdLocalFiles.SelectedItem as FileItem;
                if (f.Type == "Directory")
                {
                    if (f.Name == "...")
                    {
                        if (trvLocalFolders.SelectedNode.Parent != null)
                        {
                            trvLocalFolders.SelectedNode = trvLocalFolders.SelectedNode.Parent;
                        }
                    }
                    else
                    {
                        String argPath = Path.Combine(txtLocalPath.Text, f.Name);
                        TreeNode node = FindLocalNode(trvLocalFolders.Nodes[0], argPath);
                        if (node != null)
                        {
                            trvLocalFolders.SelectedNode = node;
                        }
                    }
                }
                else
                {
                    await ProcessAsync.StartAsync(Path.Combine(txtLocalPath.Text, (grdLocalFiles.SelectedItem as FileItem).Name));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TreeNode FindLocalNode(TreeNode argNode, String argPath)
        {
            if ((argNode.Tag as DirectoryInfo).FullName == argPath)
            {
                return argNode;
            }
            else
            {
                if (argNode.Nodes.Count > 0)
                {
                    foreach (TreeNode node in argNode.Nodes)
                    {
                        TreeNode selNode = FindLocalNode(node, argPath);
                        if (selNode != null)
                        {
                            return selNode;
                        }
                    }
                }
            }
            return null;
        }

        private TreeNode FindFtpNode(TreeNode argNode, String argPath)
        {
            if ((argNode.Tag as FtpFolder).FullPath == argPath)
            {
                return argNode;
            }
            else
            {
                if (argNode.Nodes.Count > 0)
                {
                    foreach (TreeNode node in argNode.Nodes)
                    {
                        TreeNode selNode = FindFtpNode(node, argPath);
                        if (selNode != null)
                        {
                            return selNode;
                        }
                    }
                }
            }
            return null;
        }

        private async void btnRefreshFtpPath_Click(object sender, EventArgs e)
        {
            TaskCompletionSource<int> taskCompletionSource = null;
            if (sender is TaskCompletionSource<int> t)
            {
                taskCompletionSource = t;
            }

            try
            {
                if (trvFtpFolders.SelectedNode == null)
                {
                    taskCompletionSource?.SetResult(1);
                    return;
                }

                grpFtpFiles.Enabled = false;
                grpFtpFiles.Cursor = Cursors.WaitCursor;

                // Read the Sub Directories of the selected node and update the Folder Tree View
                TreeNode selNode = trvFtpFolders.SelectedNode;
                selNode.Tag = await _ftp.GetFtpFolderDetailsAsync((selNode.Tag as FtpFolder).FullPath, 0);
                FtpFolder selDirInfo = selNode.Tag as FtpFolder;
                txtFtpPath.Text = selDirInfo.FullPath;
                GetFtpDirectories(selNode, 0);

                // Read the contents and add them to the ListView
                // Keep the sorted column and order
                DataGridViewColumn sortColumn = grdFtpFiles.SortedColumn;
                SortOrder sortOrder = grdFtpFiles.SortOrder;
                int oldVerticalOffset = grdFtpFiles.FirstDisplayedScrollingRowIndex;
                int oldSelectedIndex = grdFtpFiles.SelectedIndex;
                // First clear the list view contents
                grdFtpFiles.DataSource = null;
                // ApplyThemeToControl(grdFtpFiles, chkDarkMode.Checked); grdFtpFiles.Refresh(); // This call is removed
                ApplyThemeToDataGridView(grdFtpFiles, chkDarkMode.Checked);

                List<FileItem> items = new List<FileItem>();

                // First add the Up Directory only if there is a parent node
                if (selNode.Parent != null)
                {
                    FileItem itemUp = new FileItem
                    {
                        Type = "Directory",
                        Name = "...",
                        Icon = Properties.Resources.folder1.ToBitmap(),
                        Size = null
                    };
                    items.Add(itemUp);
                }

                IList<FtpFolder> localDirs = null;
                try
                {
                    localDirs = selDirInfo.Folders;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localDirs != null)
                {
                    foreach (FtpFolder dir in localDirs)
                    {
                        FileItem item = new FileItem
                        {
                            Type = "Directory",
                            Name = dir.Name,
                            Icon = Properties.Resources.folder1.ToBitmap(),
                            Date = dir.Date,
                            Size = null
                        };
                        items.Add(item);
                    }
                }

                IList<FtpFile> localFiles = null;
                try
                {
                    localFiles = selDirInfo.Files;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                if (localFiles != null)
                {
                    foreach (FtpFile file in localFiles)
                    {
                        FileItem item = new FileItem
                        {
                            Type = file.Extension(),
                            Name = file.Name,
                            Icon = IconReader.GetFileIcon(file.Extension(), IconReader.IconSize.Small, false).ToBitmap(),
                            Date = file.Date,
                            Size = new gFtpGUI.FileSize(file.Size)
                        };
                        items.Add(item);
                    }
                }

                grdFtpFiles.DataSource = items;
                ApplyThemeToDataGridView(grdFtpFiles, chkDarkMode.Checked);
                // ApplyThemeToDataGridView already calls Refresh, so no need for ApplyThemeToControl or grdFtpFiles.Refresh()
                // Set the sortedColumn and order again
                if (sortColumn != null)
                {
                    // Find the new DataGridViewColumn
                    DataGridViewColumn newSortColumn = grdFtpFiles.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == sortColumn.Name);
                    if (newSortColumn != null)
                    {
                        if (sortOrder == SortOrder.Ascending)
                        {
                            grdFtpFiles.Sort(newSortColumn, ListSortDirection.Ascending);
                        }
                        else if (sortOrder == SortOrder.Descending)
                        {
                            grdFtpFiles.Sort(newSortColumn, ListSortDirection.Descending);
                        }
                        ApplyThemeToDataGridView(grdFtpFiles, chkDarkMode.Checked);
                    }
                }

                // Check if Vertical Offset is now valid
                if (oldVerticalOffset > grdFtpFiles.Rows.Count - 1)
                {
                    oldVerticalOffset = grdFtpFiles.Rows.Count - 1;
                }

                grdFtpFiles.FirstDisplayedScrollingRowIndex = oldVerticalOffset;


                // Check if selected index is now valid
                if (oldSelectedIndex > grdFtpFiles.Rows.Count - 1)
                {
                    oldSelectedIndex = grdFtpFiles.Rows.Count - 1;
                }

                grdFtpFiles.SetSelectedRowByIndex(oldSelectedIndex, false);

                grpFtpFiles.Enabled = true;

                grdFtpFiles.PerformLayout();

                grpFtpFiles.Cursor = Cursors.Default;

                taskCompletionSource?.SetResult(1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                grpFtpFiles.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                grpFtpFiles.Enabled = true;

                taskCompletionSource?.SetResult(1);
            }
        }

        private void cmbFtpConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearFtpFolder();
                if (cmbFtpConnections.SelectedItem == null)
                {
                    txtFtpServer.Clear();
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
                else
                {
                    FtpConnection myCon = cmbFtpConnections.SelectedItem as FtpConnection;
                    txtFtpServer.Text = myCon.Server;
                    txtUsername.Text = myCon.Username;
                    txtPassword.Text = myCon.Password;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveFtpConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtFtpServer.Text))
                {
                    throw new Exception("The ftp server cannot be empty!");
                }
                if (String.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    throw new Exception("The ftp username cannot be empty!");
                }
                if (String.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    throw new Exception("The ftp password cannot be empty!");
                }

                if (
                    (cmbFtpConnections.DataSource as IList<FtpConnection>)
                    .Any(f =>
                        f.Server.ToLower().Trim().Equals(txtFtpServer.Text.ToLower().Trim())
                        && f.Username.ToLower().Trim().Equals(txtUsername.Text.ToLower().Trim())
                    )
                )
                {
                    throw new Exception("The FTP connection already exists!");
                }

                FtpConnection newCon = new FtpConnection
                {
                    Server = txtFtpServer.Text.Trim(),
                    Username = txtUsername.Text.Trim(),
                    Password = txtPassword.Text.Trim()
                };

                List<FtpConnection> newList = cmbFtpConnections.DataSource as List<FtpConnection>;
                newList.Add(newCon);

                String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connections.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<FtpConnection>));
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    serializer.Serialize(fs, newList);
                }

                InitFtpConnections();
                cmbFtpConnections.SelectedItem = (cmbFtpConnections.Items as IList<FtpConnection>).FirstOrDefault(t => t.Server == newCon.Server);

                MessageBox.Show("The connection was saved!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteFtpConnection_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if an FtpConnections was selected
                if (cmbFtpConnections.SelectedIndex == -1)
                {
                    return;
                }

                // First ask the user
                if (MessageBox.Show(
                    "Do you really want to delete the selected Ftp Connection?",
                    "Are you sure?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Get the selected index
                int prevSelectionIndex = cmbFtpConnections.SelectedIndex;

                // Get the items as list
                List<FtpConnection> connectionsList = cmbFtpConnections.DataSource as List<FtpConnection>;

                // Remove the selected FtpConnection from the list
                connectionsList.Remove(cmbFtpConnections.SelectedItem as FtpConnection);

                // Update the file with the ftp connections
                String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "connections.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<FtpConnection>));
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    serializer.Serialize(fs, connectionsList);
                }

                // Read the ftp connections from the file
                InitFtpConnections();

                // Check if the previous selected index is now valid
                if (prevSelectionIndex > cmbFtpConnections.Items.Count - 1)
                {
                    prevSelectionIndex = cmbFtpConnections.Items.Count - 1;
                }

                cmbFtpConnections.SelectedIndex = prevSelectionIndex;

                MessageBox.Show("The connection was deleted!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveAriaPath_Click(object sender, EventArgs e)
        {
            try
            {
                String filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.ini");
                _settings.Aria2Path = txtAria.Text.Trim();
                gSettingsManager.SaveSettings(_settings, filename);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnJobs_Click(object sender, EventArgs e)
        {
            try
            {
                _FrmQueue.WindowState = FormWindowState.Normal;
                _FrmQueue.Show();
                _FrmQueue.Select();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLocalPathOpen_Click(object sender, EventArgs e)
        {
            try
            {
                string localPath = txtLocalPath.Text;

                if (string.IsNullOrWhiteSpace(localPath))
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                if (await DirectoryAsync.ExistsAsync(localPath))
                {
                    await ProcessAsync.StartAsync("explorer.exe", localPath);
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLocalPathCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string localPath = txtLocalPath.Text;

                if (string.IsNullOrWhiteSpace(localPath))
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                if (!await DirectoryAsync.ExistsAsync(localPath))
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                this.Cursor = Cursors.Default;

                string subFolder = gInputForm.InputBox($"Please write the sub folder name for {localPath}:", "Create a new sub folder", multiLineText: false);
                if (string.IsNullOrWhiteSpace(subFolder))
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                string finalPath = Path.Combine(localPath, subFolder);

                if (await DirectoryAsync.ExistsAsync(finalPath))
                {
                    throw new Exception($"The final path {finalPath} already exists!");
                }

                DirectoryInfo finalDirInfo = await DirectoryAsync.CreateDirectoryAsync(finalPath);

                if (finalDirInfo == null)
                {
                    throw new Exception($"The final path {finalPath} could not be created!");
                }

                btnRefreshLocalPath_Click(sender, e);

                TreeNode finalNode = FindLocalNode(trvLocalFolders.SelectedNode, finalPath);
                while (finalNode == null)
                {
                    Application.DoEvents();
                    finalNode = FindLocalNode(trvLocalFolders.SelectedNode, finalPath);
                }

                trvLocalFolders.SelectedNode = finalNode;

                MessageBox.Show($"{finalPath} was created!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLocalPathDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string localPath = txtLocalPath.Text;

                if (string.IsNullOrWhiteSpace(localPath))
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                if (!await DirectoryAsync.ExistsAsync(localPath))
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                this.Cursor = Cursors.Default;

                DialogResult result = MessageBox.Show($"Do you really want to delete folder: {localPath}?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.No:
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        break;
                }

                this.Cursor = Cursors.WaitCursor;

                await DirectoryAsync.DeleteAsync(localPath);

                trvLocalFolders.SelectedNode = trvLocalFolders.SelectedNode.Parent;

                MessageBox.Show($"{localPath} was deleted!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLocalPathRename_Click(object sender, EventArgs e)
        {
            try
            {
                string localPath = txtLocalPath.Text;

                if (string.IsNullOrWhiteSpace(localPath))
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                if (!await DirectoryAsync.ExistsAsync(localPath))
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                this.Cursor = Cursors.Default;

                string newFolderName = gInputForm.InputBox($"Please write the new folder name for {localPath}:", "Rename an existing folder", multiLineText: false);
                if (string.IsNullOrWhiteSpace(newFolderName))
                {
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                string parentFolder = (await DirectoryAsync.GetParentAsync(localPath)).FullName;

                string finalPath = Path.Combine(parentFolder, newFolderName);

                if (await DirectoryAsync.ExistsAsync(finalPath))
                {
                    throw new Exception($"The final path {finalPath} already exists!");
                }

                await DirectoryAsync.MoveAsync(localPath, finalPath);

                trvLocalFolders.SelectedNode = trvLocalFolders.SelectedNode.Parent;

                btnRefreshLocalPath_Click(sender, e);

                TreeNode finalNode = FindLocalNode(trvLocalFolders.SelectedNode, finalPath);
                while (finalNode == null)
                {
                    Application.DoEvents();
                    finalNode = FindLocalNode(trvLocalFolders.SelectedNode, finalPath);
                }

                trvLocalFolders.SelectedNode = finalNode;

                MessageBox.Show($"{localPath} was renamed to {finalPath}!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "An error has occured!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
