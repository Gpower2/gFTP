using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Linq;

namespace gpower2.gSettings
{
    public class gSettingsManager
    {
        private static CultureInfo _InvariantCulture = CultureInfo.InvariantCulture;

        public static void ReadSettings(Object argSettingsObject, String argSettingsFilePath)
        {
            // Check if file exists
            if (!File.Exists(argSettingsFilePath))
            {
                // Write a default empty file
                SaveSettings(argSettingsObject, argSettingsFilePath);
                return;
                //throw new FileNotFoundException(String.Format("The settings file {0} was not found!", argSettingsFilePath));
            }
            // Open our settings file
            using (StreamReader sr = new StreamReader(argSettingsFilePath))
            {
                // Read settings file
                while (!sr.EndOfStream)
                {
                    // Get line
                    String line = sr.ReadLine();
                    // Check for property value
                    foreach (PropertyInfo prop in argSettingsObject.GetType().GetProperties())
                    {
                        // Check if the property can be written to
                        if (!prop.CanWrite)
                        {
                            continue;
                        }
                        // Check for property value
                        if (line.StartsWith(String.Format("{0}:", prop.Name)))
                        {
                            // Get value as String
                            String stringValue = line.Substring(line.IndexOf(":") + 1);
                            // Convert the String value to the according Type
                            Object objectValue = null;
                            if (prop.PropertyType == typeof(String))
                            {
                                objectValue = stringValue;
                            }
                            else if (prop.PropertyType == typeof(Char))
                            {
                                objectValue = Char.Parse(stringValue);
                            }
                            else if (prop.PropertyType == typeof(Int16))
                            {
                                objectValue = Int16.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(Int32))
                            {
                                objectValue = Int32.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(Int64))
                            {
                                objectValue = Int64.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(UInt16))
                            {
                                objectValue = UInt16.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(UInt32))
                            {
                                objectValue = UInt32.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(UInt64))
                            {
                                objectValue = UInt64.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(Single))
                            {
                                objectValue = Single.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(Double))
                            {
                                objectValue = Double.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(Decimal))
                            {
                                objectValue = Decimal.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(Boolean))
                            {
                                objectValue = Boolean.Parse(stringValue);
                            }
                            else if (prop.PropertyType == typeof(Byte))
                            {
                                objectValue = Byte.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(SByte))
                            {
                                objectValue = SByte.Parse(stringValue, NumberStyles.Any, _InvariantCulture);
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                objectValue = DateTime.ParseExact(stringValue, "yyyy/MM/dd HH:mm:ss.fffffff", _InvariantCulture, DateTimeStyles.AssumeUniversal);
                            }
                            else if (prop.PropertyType == typeof(TimeSpan))
                            {
                                objectValue = new TimeSpan(Int64.Parse(stringValue, NumberStyles.Any, _InvariantCulture));
                            }
                            else if (prop.PropertyType == typeof(Color))
                            {
                                objectValue = Color.FromArgb(Int32.Parse(stringValue, NumberStyles.Any, _InvariantCulture));
                            }
                            else if (prop.PropertyType == typeof(Font))
                            {
                                // Get font elements
                                String[] fontElements = stringValue.Split(new String[] { "," }, StringSplitOptions.None);
                                // Create Font
                                objectValue = new Font(
                                    fontElements[0],
                                    Single.Parse(fontElements[1], NumberStyles.Any, _InvariantCulture),
                                    (FontStyle)Enum.Parse(typeof(FontStyle), fontElements[2], true)
                                    , (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), fontElements[3], true)
                                    , Byte.Parse(fontElements[4], NumberStyles.Any, _InvariantCulture)
                                    , Boolean.Parse(fontElements[5]));
                            }

                            // Set the value if not null
                            if (objectValue != null)
                            {
                                prop.SetValue(argSettingsObject, objectValue, null);
                            }
                        }
                    }
                }
            }
        }

        public static void SaveSettings(Object argSettingsObject, String argSettingsFilePath)
        {
            StringBuilder settingsBuilder = new StringBuilder();
            String settingFormat = "{0}:{1}\r\n";
            foreach (PropertyInfo prop in argSettingsObject.GetType().GetProperties())
            {
                // Check if the property can be read
                if (!prop.CanRead)
                {
                    continue;
                }
                // Get property value
                Object objectValue = prop.GetValue(argSettingsObject, null);
                String stringValue = String.Empty;

                if (prop.PropertyType == typeof(String)
                    || prop.PropertyType == typeof(Char))
                {
                    stringValue = objectValue.IsNull("").ToString();
                }
                else if (prop.PropertyType == typeof(Int16)
                    || prop.PropertyType == typeof(Int32)
                    || prop.PropertyType == typeof(Int64))
                {
                    stringValue = Convert.ToInt64(objectValue.IsNull(-1)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(UInt16)
                    || prop.PropertyType == typeof(UInt32)
                    || prop.PropertyType == typeof(UInt64))
                {
                    stringValue = Convert.ToUInt64(objectValue.IsNull(0)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(Single)
                    || prop.PropertyType == typeof(Double))
                {
                    stringValue = Convert.ToDouble(objectValue.IsNull(-1)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(Decimal))
                {
                    stringValue = ((Decimal)objectValue.IsNull(-1)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(Boolean))
                {
                    stringValue = ((Boolean)objectValue.IsNull(false)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(Byte))
                {
                    stringValue = ((Byte)objectValue.IsNull(0)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(SByte))
                {
                    stringValue = ((SByte)objectValue.IsNull(0)).ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    stringValue = ((DateTime)objectValue.IsNull(new DateTime())).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss.fffffff", _InvariantCulture);
                }
                else if (prop.PropertyType == typeof(TimeSpan))
                {
                    stringValue = ((TimeSpan)objectValue.IsNull(new TimeSpan())).Ticks.ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(Color))
                {
                    stringValue = ((Color)objectValue.IsNull(Color.Transparent)).ToArgb().ToString(_InvariantCulture);
                }
                else if (prop.PropertyType == typeof(Font))
                {
                    stringValue = String.Format("{0},{1},{2},{3},{4},{5}",
                        ((Font)objectValue.IsNull(SystemFonts.DefaultFont)).FontFamily.Name,
                        ((Font)objectValue.IsNull(SystemFonts.DefaultFont)).Size.ToString(_InvariantCulture),
                        ((Font)objectValue.IsNull(SystemFonts.DefaultFont)).Style.ToString(),
                        ((Font)objectValue.IsNull(SystemFonts.DefaultFont)).Unit.ToString(),
                        ((Font)objectValue.IsNull(SystemFonts.DefaultFont)).GdiCharSet.ToString(_InvariantCulture),
                        ((Font)objectValue.IsNull(SystemFonts.DefaultFont)).GdiVerticalFont.ToString(_InvariantCulture)
                        );
                }
                settingsBuilder.AppendFormat(settingFormat, prop.Name, stringValue);
            }
            // Remove the trailing \r\n 
            if (settingsBuilder.Length > 1)
            {
                settingsBuilder.Length -= 2;
            }
            using (StreamWriter sw = new StreamWriter(argSettingsFilePath))
            {
                sw.Write(settingsBuilder.ToString());
            }
        }
    }
}
