using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gFtpGUI;

namespace gFTP.Tests
{
    [TestClass]
    public class ThemeManagerTests
    {
        [TestMethod]
        public void ApplyTheme_DarkMode_SetsFormBackColor()
        {
            // Arrange
            using (var form = new Form())
            {
                // Act
                ThemeManager.ApplyTheme(form, true);

                // Assert
                Assert.AreEqual(ThemeManager.DarkModeFormBackColor, form.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeFormForeColor, form.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_LightMode_SetsFormBackColor()
        {
            // Arrange
            using (var form = new Form())
            {
                // Act
                ThemeManager.ApplyTheme(form, false);

                // Assert
                Assert.AreEqual(ThemeManager.LightModeFormBackColor, form.BackColor);
                Assert.AreEqual(ThemeManager.LightModeFormForeColor, form.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_SetsButtonStyle()
        {
            // Arrange
            using (var form = new Form())
            {
                var button = new Button();
                form.Controls.Add(button);

                // Act
                ThemeManager.ApplyTheme(form, true);

                // Assert
                Assert.AreEqual(FlatStyle.Flat, button.FlatStyle);
                Assert.AreEqual(ThemeManager.DarkModeButtonBackColor, button.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeButtonForeColor, button.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_SetsTextBoxStyle()
        {
            // Arrange
            using (var form = new Form())
            {
                var textBox = new TextBox();
                form.Controls.Add(textBox);

                // Act
                ThemeManager.ApplyTheme(form, true);

                // Assert
                Assert.AreEqual(ThemeManager.DarkModeTextBackColor, textBox.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeTextForeColor, textBox.ForeColor);
                Assert.AreEqual(BorderStyle.FixedSingle, textBox.BorderStyle);
            }
        }

        [TestMethod]
        public void ApplyThemeToDataGridView_DarkMode_SetsGridColors()
        {
            // Arrange
            var dgv = new DataGridView();

            // Act
            ThemeManager.ApplyThemeToDataGridView(dgv, true);

            // Assert
            Assert.AreEqual(ThemeManager.DarkModeGridBackColor, dgv.BackgroundColor);
            Assert.AreEqual(ThemeManager.DarkModeGridCellBackColor, dgv.DefaultCellStyle.BackColor);
            Assert.AreEqual(ThemeManager.DarkModeGridForeColor, dgv.DefaultCellStyle.ForeColor);
            Assert.AreEqual(ThemeManager.DarkModeGridHeaderBackColor, dgv.ColumnHeadersDefaultCellStyle.BackColor);
            Assert.IsFalse(dgv.EnableHeadersVisualStyles);
        }

        [TestMethod]
        public void ApplyThemeToDataGridView_LightMode_SetsGridColors()
        {
            // Arrange
            var dgv = new DataGridView();

            // Act
            ThemeManager.ApplyThemeToDataGridView(dgv, false);

            // Assert
            Assert.AreEqual(SystemColors.Window, dgv.BackgroundColor);
            Assert.AreEqual(ThemeManager.LightModeGridCellBackColor, dgv.DefaultCellStyle.BackColor);
            Assert.AreEqual(SystemColors.WindowText, dgv.DefaultCellStyle.ForeColor);
            Assert.AreEqual(Color.White, dgv.ColumnHeadersDefaultCellStyle.BackColor);
            Assert.IsTrue(dgv.EnableHeadersVisualStyles);
        }

        [TestMethod]
        public void ApplyThemeToDataGridView_DarkMode_PreservesThemeAfterDataSourceChange()
        {
            // Arrange
            var dgv = new DataGridView();
            ThemeManager.ApplyThemeToDataGridView(dgv, true);

            // Act - Simulate adding data (which might cause theme loss in some cases)
            dgv.DataSource = new[] { new { Name = "Test1" }, new { Name = "Test2" } };

            // Assert - Theme should still be applied
            Assert.AreEqual(ThemeManager.DarkModeGridBackColor, dgv.BackgroundColor);
            Assert.AreEqual(ThemeManager.DarkModeGridCellBackColor, dgv.DefaultCellStyle.BackColor);
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_SetsLabelTransparent()
        {
            // Arrange
            using (var form = new Form())
            {
                var label = new Label();
                form.Controls.Add(label);

                // Act
                ThemeManager.ApplyTheme(form, true);

                // Assert
                Assert.AreEqual(Color.Transparent, label.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeContainerForeColor, label.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_SetsGroupBoxColor()
        {
            // Arrange
            using (var form = new Form())
            {
                var groupBox = new GroupBox();
                form.Controls.Add(groupBox);

                // Act
                ThemeManager.ApplyTheme(form, true);

                // Assert
                Assert.AreEqual(ThemeManager.DarkModeContainerBackColor, groupBox.BackColor);
                Assert.AreEqual(ThemeManager.DarkModeContainerForeColor, groupBox.ForeColor);
            }
        }

        [TestMethod]
        public void ApplyTheme_DarkMode_RecursesToChildren()
        {
            // Arrange
            using (var form = new Form())
            {
                var panel = new Panel();
                var button = new Button();
                panel.Controls.Add(button);
                form.Controls.Add(panel);

                // Act
                ThemeManager.ApplyTheme(form, true);

                // Assert
                Assert.AreEqual(ThemeManager.DarkModeContainerBackColor, panel.BackColor);
                Assert.AreEqual(FlatStyle.Flat, button.FlatStyle);
            }
        }

        [TestMethod]
        public void ApplyTheme_NullControl_DoesNotThrow()
        {
            // Act & Assert - should not throw
            ThemeManager.ApplyTheme(null, true);
        }

        [TestMethod]
        public void ApplyThemeToDataGridView_DarkMode_NoAlternatingRows()
        {
            // Arrange
            var dgv = new DataGridView();

            // Act
            ThemeManager.ApplyThemeToDataGridView(dgv, true);

            // Assert - All rows should have the same dark theme (no alternating colors)
            Assert.AreEqual(ThemeManager.DarkModeGridCellBackColor, dgv.DefaultCellStyle.BackColor);
            Assert.AreEqual(ThemeManager.DarkModeGridCellBackColor, dgv.AlternatingRowsDefaultCellStyle.BackColor);
            Assert.AreEqual(ThemeManager.DarkModeGridForeColor, dgv.DefaultCellStyle.ForeColor);
            Assert.AreEqual(ThemeManager.DarkModeGridForeColor, dgv.AlternatingRowsDefaultCellStyle.ForeColor);
        }

        [TestMethod]
        public void ApplyThemeToDataGridView_NullGrid_DoesNotThrow()
        {
            // Act & Assert - should not throw
            ThemeManager.ApplyThemeToDataGridView(null, true);
        }
    }
}
