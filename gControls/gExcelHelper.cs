using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using System.Reflection;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml.ConditionalFormatting;
using System.Collections;
using System.Windows.Forms;

namespace gpower2.gControls
{
    public static class gExcelHelper
    {
        public static Stream DataSetToExcelStream(DataSet data)
        {
            using (ExcelPackage excelFile = DataSetToExcelPackage(data))
            {
                return new MemoryStream(excelFile.GetAsByteArray());
            }
        }

        public static void DataSetToExcelFile(DataSet data, String filename)
        {
            using (ExcelPackage excelFile = DataSetToExcelPackage(data))
            {
                excelFile.SaveAs(new FileInfo(filename));
            }
        }

        public static void ListToExcelFile(IList data, String filename)
        {
            using (ExcelPackage excelFile = ListToExcelPackage(data))
            {
                excelFile.SaveAs(new FileInfo(filename));
            }
        }

        private static ExcelPackage ListToExcelPackage(IList data)
        {
            ExcelPackage excelFile = new ExcelPackage();

            // Προσθέτω ένα καινούριο WorkSheet
            String worksheetName = String.IsNullOrWhiteSpace(data.GetType().Name) ? "Στοιχεία" : data.GetType().Name;

            // Προσθέτω το worksheet
            ExcelWorksheet workSheet = excelFile.Workbook.Worksheets.Add(worksheetName);

            workSheet.View.ShowGridLines = true;

            workSheet.Row(1).Height = 35;

            Int32 logoRowOffset = 2;

            workSheet.Row(logoRowOffset + 1).Height = 35;

            // Φτιάχνω τα κελιά
            var properties = data.GetType().GetGenericArguments()[0].GetProperties();
            for (Int32 y = 0; y < properties.Length; y++)
            {
                workSheet.Column(y + 1).OutlineLevel = 0;

                // Κεφαλίδες
                workSheet.Cells[logoRowOffset + 1, y + 1].Value = properties[y].Name;
                workSheet.Cells[logoRowOffset + 1, y + 1].Style.Font.Bold = true;
                workSheet.Cells[logoRowOffset + 1, y + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[logoRowOffset + 1, y + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[logoRowOffset + 1, y + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                workSheet.Cells[logoRowOffset + 1, y + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[logoRowOffset + 1, y + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Δεδομένα
                for (Int32 x = 0; x < data.Count; x++)
                {
                    if (properties[y].PropertyType == typeof(DateTime))
                    {
                        if (properties[y].GetValue(data[x], null) != null)
                        {
                            workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = ((DateTime)properties[y].GetValue(data[x], null)).ToString("dd/MM/yyyy");
                            workSheet.Cells[logoRowOffset + x + 2, y + 1].Style.Numberformat.Format = "dd/MM/yyyy";
                        }
                    }
                    else if (properties[y].PropertyType == typeof(Decimal) ||
                        properties[y].PropertyType == typeof(Double) ||
                        properties[y].PropertyType == typeof(Single))
                    {
                        workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = properties[y].GetValue(data[x], null);
                        workSheet.Cells[logoRowOffset + x + 2, y + 1].Style.Numberformat.Format = "#,##0.00";
                    }
                    else if (properties[y].PropertyType == typeof(Boolean))
                    {
                        if (properties[y].GetValue(data[x], null) != null)
                        {
                            workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = (Boolean)properties[y].GetValue(data[x], null) ? "Ναι" : "Όχι";
                        }
                    }
                    else
                    {
                        workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = properties[y].GetValue(data[x], null);
                    }

                    workSheet.Cells[logoRowOffset + x + 2, y + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                workSheet.Column(y + 1).AutoFit();
                workSheet.Column(y + 1).Width += 1;
            }

            // Logo
            // Παίρνω το logo του gPharmacy
            Image logo = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();

            // Αν βρέθηκε το logo, το προσθέτω στην κορυφή του worksheet
            if (logo != null)
            {
                ExcelPicture logoPicture = workSheet.Drawings.AddPicture("logo", logo);
                logoPicture.SetPosition(0, 3, 0, 3);
            }

            return excelFile;
        }

        private static ExcelPackage DataSetToExcelPackage(DataSet data)
        {
            ExcelPackage excelFile = new ExcelPackage();

            // Για κάθε DataTable προσθέτω και ένα καινούριο WorkSheet
            for (Int32 i = 0; i < data.Tables.Count; i++)
            {
                DataTable dtTable = data.Tables[i];

                // Βρίσκω το όνομα
                String worksheetName = String.IsNullOrWhiteSpace(dtTable.TableName) ? String.Format("Table {0}", i) : dtTable.TableName;
                // Προσθέτω το worksheet
                ExcelWorksheet workSheet = excelFile.Workbook.Worksheets.Add(worksheetName);

                workSheet.View.ShowGridLines = true;

                workSheet.Row(1).Height = 35;

                Int32 logoRowOffset = 2;

                workSheet.Row(logoRowOffset + 1).Height = 35;

                // Φτιάχνω τα κελιά
                for (Int32 y = 0; y < dtTable.Columns.Count; y++)
                {
                    workSheet.Column(y + 1).OutlineLevel = 0;

                    // Κεφαλίδες
                    workSheet.Cells[logoRowOffset + 1, y + 1].Value = dtTable.Columns[y].ColumnName;
                    workSheet.Cells[logoRowOffset + 1, y + 1].Style.Font.Bold = true;
                    workSheet.Cells[logoRowOffset + 1, y + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    workSheet.Cells[logoRowOffset + 1, y + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[logoRowOffset + 1, y + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    workSheet.Cells[logoRowOffset + 1, y + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[logoRowOffset + 1, y + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Δεδομένα
                    for (Int32 x = 0; x < dtTable.Rows.Count; x++)
                    {
                        if (dtTable.Columns[y].DataType == typeof(DateTime))
                        {
                            if (dtTable.Rows[x][y] != DBNull.Value)
                            {
                                workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = ((DateTime)dtTable.Rows[x][y]).ToString("dd/MM/yyyy");
                                workSheet.Cells[logoRowOffset + x + 2, y + 1].Style.Numberformat.Format = "dd/MM/yyyy";
                            }
                        }
                        else if (dtTable.Columns[y].DataType == typeof(Decimal))
                        {
                            workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = dtTable.Rows[x][y];
                            workSheet.Cells[logoRowOffset + x + 2, y + 1].Style.Numberformat.Format = "#,##0.00";
                        }
                        else if (dtTable.Columns[y].DataType == typeof(Boolean))
                        {
                            if (dtTable.Rows[x][y] != DBNull.Value)
                            {
                                workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = (Boolean)dtTable.Rows[x][y] ? "Ναι" : "Όχι";
                            }
                        }
                        else
                        {
                            workSheet.Cells[logoRowOffset + x + 2, y + 1].Value = dtTable.Rows[x][y];
                        }

                        workSheet.Cells[logoRowOffset + x + 2, y + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    workSheet.Column(y + 1).AutoFit();
                    workSheet.Column(y + 1).Width += 1;
                }

                // Logo
                // Παίρνω το logo του gPharmacy
                Image logo = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();

                // Αν βρέθηκε το logo, το προσθέτω στην κορυφή του worksheet
                if (logo != null)
                {
                    ExcelPicture logoPicture = workSheet.Drawings.AddPicture("logo", logo);
                    logoPicture.SetPosition(0, 3, 0, 3);
                }
            }

            return excelFile;
        }
    }
}