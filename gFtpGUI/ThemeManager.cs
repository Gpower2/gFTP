using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using gpower2.gControls;

namespace gFtpGUI
{
    /// <summary>
    /// Centralized theme manager for gFTP application.
    /// Based on the gMKVExtractGUI ThemeManager reference implementation.
    /// </summary>
    public static class ThemeManager
    {
        // Define Light and Dark Colors
        // Basic Colors
        public static Color LightModeFormBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeFormForeColor { get; set; } = SystemColors.ControlText;
        public static Color LightModeContainerBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeContainerForeColor { get; set; } = SystemColors.ControlText;
        public static Color LightModeTextBackColor { get; set; } = SystemColors.Window;
        public static Color LightModeTextForeColor { get; set; } = SystemColors.WindowText;
        public static Color LightModeButtonBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeButtonForeColor { get; set; } = SystemColors.ControlText;
        public static Color LightModeMenuBackColor { get; set; } = SystemColors.Control;
        public static Color LightModeMenuForeColor { get; set; } = SystemColors.MenuText;
        public static Color LightModeGridBackColor { get; set; } = SystemColors.ControlDark;
        public static Color LightModeGridCellBackColor { get; set; } = SystemColors.Window;
        public static Color LightModeGridHeaderBackColor { get; set; } = SystemColors.Control;

        public static Color DarkModeFormBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public static Color DarkModeFormForeColor { get; set; } = Color.White;
        public static Color DarkModeContainerBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public static Color DarkModeContainerForeColor { get; set; } = Color.White;
        public static Color DarkModeTextBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public static Color DarkModeTextForeColor { get; set; } = Color.White;
        public static Color DarkModeButtonBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public static Color DarkModeButtonForeColor { get; set; } = Color.White;
        public static Color DarkModeMenuBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public static Color DarkModeMenuForeColor { get; set; } = Color.White;
        public static Color DarkModeGridBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public static Color DarkModeGridCellBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public static Color DarkModeGridHeaderBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public static Color DarkModeGridForeColor { get; set; } = Color.White;

        /// <summary>
        /// Apply theme to a control and all its children recursively.
        /// </summary>
        public static void ApplyTheme(Control control, bool darkMode)
        {
            if (control == null || control.IsDisposed)
            {
                return;
            }

            // Apply native theme (scrollbars, etc.)
            ApplyNativeTheme(control, darkMode);

            Color formBackColor = darkMode ? DarkModeFormBackColor : LightModeFormBackColor;
            Color formForeColor = darkMode ? DarkModeFormForeColor : LightModeFormForeColor;
            Color containerBackColor = darkMode ? DarkModeContainerBackColor : LightModeContainerBackColor;
            Color containerForeColor = darkMode ? DarkModeContainerForeColor : LightModeContainerForeColor;
            Color textBackColor = darkMode ? DarkModeTextBackColor : LightModeTextBackColor;
            Color textForeColor = darkMode ? DarkModeTextForeColor : LightModeTextForeColor;
            Color menuBackColor = darkMode ? DarkModeMenuBackColor : LightModeMenuBackColor;
            Color menuForeColor = darkMode ? DarkModeMenuForeColor : LightModeMenuForeColor;

            if (control is Form || control is gForm)
            {
                control.BackColor = formBackColor;
                control.ForeColor = formForeColor;
            }
            else if (control is GroupBox || control is Panel || control is TabControl)
            {
                control.BackColor = containerBackColor;
                control.ForeColor = containerForeColor;

                if (control is GroupBox groupBox)
                {
                    groupBox.Paint -= groupBoxPaintEventHandler;
                    if (darkMode)
                    {
                        groupBox.Paint += groupBoxPaintEventHandler;
                    }
                }
            }
            else if (control is TextBox || control is RichTextBox || control is gTextBox)
            {
                bool nativeThemeNeedsRefresh = false;
                control.BackColor = textBackColor;
                control.ForeColor = textForeColor;

                if (control is TextBox textBox)
                {
                    BorderStyle targetBorderStyle = darkMode ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
                    if (textBox.BorderStyle != targetBorderStyle)
                    {
                        textBox.BorderStyle = targetBorderStyle;
                        nativeThemeNeedsRefresh = true;
                    }
                }

                if (control is RichTextBox rich)
                {
                    try
                    {
                        if (darkMode)
                        {
                            rich.BackColor = rich.Parent?.BackColor ?? DarkModeTextBackColor;
                            if (rich.BorderStyle != BorderStyle.None)
                            {
                                rich.BorderStyle = BorderStyle.None;
                                nativeThemeNeedsRefresh = true;
                            }
                            rich.SelectionBackColor = Color.FromArgb(80, 80, 80);
                            rich.SelectionColor = Color.White;
                        }
                        else
                        {
                            if (rich.BorderStyle != BorderStyle.Fixed3D)
                            {
                                rich.BorderStyle = BorderStyle.Fixed3D;
                                nativeThemeNeedsRefresh = true;
                            }
                            rich.SelectionBackColor = SystemColors.Highlight;
                            rich.SelectionColor = SystemColors.HighlightText;

                            if (rich.ReadOnly)
                            {
                                control.BackColor = SystemColors.Control;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                if (nativeThemeNeedsRefresh)
                {
                    ApplyNativeTheme(control, darkMode);
                }
            }
            else if (control is Button btn)
            {
                if (darkMode)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.DarkGray;
                    btn.BackColor = DarkModeButtonBackColor;
                    btn.ForeColor = DarkModeButtonForeColor;
                    btn.UseVisualStyleBackColor = true;
                }
                else
                {
                    btn.FlatStyle = FlatStyle.Standard;
                    btn.BackColor = LightModeButtonBackColor;
                    btn.ForeColor = LightModeButtonForeColor;
                    btn.UseVisualStyleBackColor = true;
                    btn.FlatAppearance.BorderColor = SystemColors.ControlDark;
                }
                btn.Invalidate();
            }
            else if (control is CheckBox chk)
            {
                chk.BackColor = darkMode ? DarkModeContainerBackColor : LightModeContainerBackColor;
                chk.ForeColor = darkMode ? DarkModeContainerForeColor : LightModeContainerForeColor;
            }
            else if (control is RadioButton rdo)
            {
                rdo.BackColor = darkMode ? DarkModeContainerBackColor : LightModeContainerBackColor;
                rdo.ForeColor = darkMode ? DarkModeContainerForeColor : LightModeContainerForeColor;
            }
            else if (control is ComboBox cb)
            {
                NativeMethods.SetWindowThemeForComboBoxManaged(control.Handle, darkMode);

                ComboBoxStyle originalStyle = cb.DropDownStyle;
                try
                {
                    if (darkMode)
                    {
                        cb.BackColor = DarkModeTextBackColor;
                        cb.ForeColor = DarkModeFormForeColor;
                    }
                    else
                    {
                        cb.BackColor = SystemColors.Window;
                        cb.ForeColor = SystemColors.ControlText;
                    }
                    cb.Invalidate();
                }
                finally
                {
                    cb.DropDownStyle = originalStyle;
                }

                if (cb is gComboBox gcmb && gcmb.ContextMenuStrip != null)
                {
                    if (darkMode)
                    {
                        gcmb.ContextMenuStrip.BackColor = DarkModeButtonBackColor;
                        gcmb.ContextMenuStrip.ForeColor = DarkModeFormForeColor;
                        gcmb.ContextMenuStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            ApplyToolStripItemThemeForComboBox(item, darkMode);
                        }
                    }
                    else
                    {
                        gcmb.ContextMenuStrip.BackColor = SystemColors.ControlLightLight;
                        gcmb.ContextMenuStrip.ForeColor = SystemColors.ControlText;
                        gcmb.ContextMenuStrip.RenderMode = ToolStripRenderMode.System;
                        foreach (ToolStripItem item in gcmb.ContextMenuStrip.Items)
                        {
                            ApplyToolStripItemThemeForComboBox(item, darkMode);
                        }
                    }
                }
            }
            else if (control is ListBox lb)
            {
                control.BackColor = textBackColor;
                control.ForeColor = textForeColor;
            }
            else if (control is TreeView tv)
            {
                tv.BackColor = textBackColor;
                tv.ForeColor = textForeColor;
                tv.BorderStyle = BorderStyle.FixedSingle;
                tv.LineColor = darkMode ? Color.LightGray : SystemColors.ControlLight;
            }
            else if (control is DataGridView dgv)
            {
                ApplyThemeToDataGridView(dgv, darkMode);
            }
            else if (control is MenuStrip ms)
            {
                ms.BackColor = menuBackColor;
                ms.ForeColor = menuForeColor;
                foreach (ToolStripItem item in ms.Items)
                {
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is ContextMenuStrip cms)
            {
                ApplyContextMenuTheme(cms, darkMode);
            }
            else if (control is StatusStrip ss)
            {
                ss.BackColor = menuBackColor;
                ss.ForeColor = menuForeColor;
                ss.RenderMode = darkMode ? ToolStripRenderMode.ManagerRenderMode : ToolStripRenderMode.System;
                foreach (ToolStripItem item in ss.Items)
                {
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is ToolStrip ts)
            {
                ts.BackColor = menuBackColor;
                ts.ForeColor = menuForeColor;
                foreach (ToolStripItem item in ts.Items)
                {
                    ApplyToolStripItemTheme(item, darkMode);
                }
            }
            else if (control is Label)
            {
                control.BackColor = Color.Transparent;
                control.ForeColor = darkMode ? DarkModeContainerForeColor : LightModeContainerForeColor;
            }
            else if (control is ProgressBar pb)
            {
                pb.ForeColor = darkMode ? Color.FromArgb(0, 122, 204) : SystemColors.Highlight;
            }
            else if (control.HasChildren
                && !(control is Form
                || control is gForm
                || control is GroupBox
                || control is Panel
                || control is TabControl))
            {
                control.BackColor = containerBackColor;
                control.ForeColor = containerForeColor;
            }

            // Recursively apply to children
            foreach (Control childControl in control.Controls)
            {
                ApplyTheme(childControl, darkMode);
            }
        }

        /// <summary>
        /// Apply theme specifically to a DataGridView.
        /// This method is public so it can be called directly when data changes.
        /// </summary>
        public static void ApplyThemeToDataGridView(DataGridView dgv, bool darkMode)
        {
            if (dgv == null || dgv.IsDisposed)
            {
                return;
            }

            dgv.BackgroundColor = darkMode ? DarkModeGridBackColor : SystemColors.Window;
            dgv.BackColor = darkMode ? DarkModeGridBackColor : SystemColors.Window;
            dgv.GridColor = darkMode ? Color.Gray : SystemColors.ControlDarkDark;

            if (darkMode)
            {
                dgv.DefaultCellStyle.BackColor = DarkModeGridCellBackColor;
                dgv.DefaultCellStyle.ForeColor = DarkModeGridForeColor;
                dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                dgv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                // No alternating rows - use same dark theme for all rows to reduce visual clatter
                dgv.AlternatingRowsDefaultCellStyle.BackColor = DarkModeGridCellBackColor;
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = DarkModeGridForeColor;
                dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkModeGridHeaderBackColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = DarkModeGridForeColor;
                dgv.RowHeadersDefaultCellStyle.BackColor = DarkModeGridHeaderBackColor;
                dgv.RowHeadersDefaultCellStyle.ForeColor = DarkModeGridForeColor;

                dgv.EnableHeadersVisualStyles = false;
            }
            else
            {
                dgv.DefaultCellStyle.BackColor = LightModeGridCellBackColor;
                dgv.DefaultCellStyle.ForeColor = SystemColors.WindowText;
                dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                dgv.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
                dgv.RowHeadersDefaultCellStyle.BackColor = LightModeGridHeaderBackColor;
                dgv.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;

                dgv.EnableHeadersVisualStyles = true;
            }

            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
            dgv.RowHeadersDefaultCellStyle.SelectionBackColor = dgv.RowHeadersDefaultCellStyle.BackColor;
            dgv.RowHeadersDefaultCellStyle.SelectionForeColor = dgv.RowHeadersDefaultCellStyle.ForeColor;

            // Set dark mode flag on gDataGridView for scrollbar corner theming
            if (dgv is gpower2.gControls.gDataGridView gdgv)
            {
                gdgv.DarkModeEnabled = darkMode;
            }

            // Apply theme to context menu if present
            if (dgv.ContextMenuStrip != null)
            {
                ApplyContextMenuTheme(dgv.ContextMenuStrip, darkMode);
            }
        }

        private static void ApplyNativeTheme(Control control, bool darkMode)
        {
            ApplyNativeThemeCore(control, darkMode);
        }

        private static void ApplyNativeThemeCore(Control control, bool darkMode)
        {
            if (control is ToolStripDropDown)
            {
                return;
            }

            // Retheming popup menu HWNDs during Opening can corrupt native menu state.
            NativeMethods.SetWindowThemeManaged(control.Handle, darkMode);
            NativeMethods.TrySetImmersiveDarkMode(control.Handle, darkMode);
        }

        public static void ApplyToolStripItemTheme(ToolStripItem item, bool darkMode)
        {
            if (darkMode)
            {
                item.BackColor = DarkModeMenuBackColor;
                item.ForeColor = DarkModeMenuForeColor;
            }
            else
            {
                if (item is ToolStripStatusLabel statusLabel)
                {
                    statusLabel.BackColor = Color.Transparent;
                    statusLabel.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    item.BackColor = SystemColors.ControlLightLight;
                    item.ForeColor = SystemColors.ControlText;
                }
            }

            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
            {
                ApplyContextMenuTheme(menuItem.DropDown, darkMode);
            }
        }

        public static void ApplyContextMenuTheme(ToolStripDropDown menu, bool darkMode)
        {
            if (menu == null || menu.IsDisposed)
            {
                return;
            }

            if (menu is ToolStripDropDownMenu dropDownMenu)
            {
                dropDownMenu.ShowCheckMargin = false;
                dropDownMenu.ShowImageMargin = true;
            }

            if (darkMode)
            {
                menu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                menu.BackColor = DarkModeMenuBackColor;
                menu.ForeColor = DarkModeMenuForeColor;
            }
            else
            {
                menu.RenderMode = ToolStripRenderMode.Professional;
                menu.BackColor = SystemColors.ControlLightLight;
                menu.ForeColor = SystemColors.ControlText;
            }

            foreach (ToolStripItem item in menu.Items)
            {
                ApplyToolStripItemTheme(item, darkMode);
            }

            menu.Invalidate();
        }

        private static void ApplyToolStripItemThemeForComboBox(ToolStripItem item, bool darkMode)
        {
            if (darkMode)
            {
                item.BackColor = DarkModeButtonBackColor;
                item.ForeColor = DarkModeFormForeColor;
            }
            else
            {
                item.BackColor = SystemColors.ControlLightLight;
                item.ForeColor = SystemColors.ControlText;
            }

            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
            {
                if (darkMode)
                {
                    menuItem.DropDown.BackColor = DarkModeButtonBackColor;
                }
                else
                {
                    menuItem.DropDown.BackColor = SystemColors.ControlLightLight;
                }
                foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                {
                    ApplyToolStripItemThemeForComboBox(dropDownItem, darkMode);
                }
            }
        }

        private static void groupBoxPaintEventHandler(object sender, PaintEventArgs e)
        {
            var groupBox = sender as GroupBox;
            if (groupBox != null && groupBox.Enabled == false)
            {
                using (Brush brush = new SolidBrush(groupBox.ForeColor))
                {
                    e.Graphics.DrawString(
                        groupBox.Text,
                        groupBox.Font,
                        brush,
                        new PointF(
                            groupBox.Font.SizeInPoints,
                            -1),
                        StringFormat.GenericTypographic);
                }
            }
        }
    }
}
