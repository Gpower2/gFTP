using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gpower2.gControls
{
    public static class ClipboardExtension
    {
        public static String GetClipboardTextFromProperty(this Object myProp)
        {
            return myProp.GetClipboardTextFromProperty(System.Globalization.CultureInfo.InstalledUICulture);
        }

        public static String GetClipboardTextFromProperty(this Object myProp, System.Globalization.CultureInfo cInfo, String argIntFormat = null, String argDecimalFormat = null, String argDateFormat = null)
        {
            if (myProp == null)
            {
                return String.Empty;
            }
            else
            {
                String rawString;
                if (myProp.GetType() == typeof(String))
                {
                    rawString = (String)myProp;
                }
                else if (myProp.GetType() == typeof(Char))
                {
                    rawString = Convert.ToString((Char)myProp);
                }
                else if (myProp.GetType() == typeof(Int16)
                    || myProp.GetType() == typeof(Int32)
                    || myProp.GetType() == typeof(Int64)
                    )
                {
                    if (String.IsNullOrWhiteSpace(argIntFormat))
                    {
                        rawString = Convert.ToInt64(myProp).ToString(cInfo);
                    }
                    else
                    {
                        rawString = Convert.ToInt64(myProp).ToString(argIntFormat, cInfo);
                    }
                }
                else if (myProp.GetType() == typeof(UInt16)
                    || myProp.GetType() == typeof(UInt32)
                    || myProp.GetType() == typeof(UInt64)
                    )
                {
                    if (String.IsNullOrWhiteSpace(argIntFormat))
                    {
                        rawString = Convert.ToUInt64(myProp).ToString(cInfo);
                    }
                    else
                    {
                        rawString = Convert.ToUInt64(myProp).ToString(argIntFormat, cInfo);
                    }
                }
                else if (myProp.GetType() == typeof(Single)
                    || myProp.GetType() == typeof(Double)
                    )
                {
                    if (String.IsNullOrWhiteSpace(argIntFormat))
                    {
                        rawString = Convert.ToDouble(myProp).ToString(cInfo);
                    }
                    else
                    {
                        rawString = Convert.ToDouble(myProp).ToString(argDecimalFormat, cInfo);
                    }
                }
                else if (myProp.GetType() == typeof(Decimal)
                    )
                {
                    if (String.IsNullOrWhiteSpace(argIntFormat))
                    {
                        rawString = ((Decimal)myProp).ToString(cInfo);
                    }
                    else
                    {
                        rawString = ((Decimal)myProp).ToString(argDecimalFormat, cInfo);
                    }
                }
                else if (myProp.GetType() == typeof(DateTime))
                {
                    if (String.IsNullOrWhiteSpace(argIntFormat))
                    {
                        rawString = ((DateTime)myProp).ToString(cInfo.DateTimeFormat.ShortDatePattern);
                    }
                    else
                    {
                        rawString = ((DateTime)myProp).ToString(argDateFormat, cInfo);
                    }
                }
                else if (myProp.GetType() == typeof(Boolean))
                {
                    rawString = ((Boolean)myProp).ToString();
                }
                else if (myProp.GetType() == typeof(Byte))
                {
                    rawString = ((Byte)myProp).ToString(cInfo);
                }
                else if (myProp is Enum)
                {
                    rawString = Enum.GetName(myProp.GetType(), myProp);
                }
                else if (myProp.GetType()
                     .GetInterfaces()
                     .Any(t => t.IsGenericType
                            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    rawString = String.Format("{{ {0} }}", (myProp as IEnumerable<Object>).GetClipboardText(cInfo, false, ","));
                }
                else
                {
                    rawString = String.Format("{{ {0} }}", myProp.GetClipboardTextFromObject(cInfo, ","));
                }
                return rawString
                    .Replace("\t", String.Empty)
                    .Replace("\r", String.Empty)
                    .Replace("\n", String.Empty)
                    .Replace(";", String.Empty);
            }            
        }

        public static String GetClipboardText<T>(this IEnumerable<T> myList, System.Globalization.CultureInfo cInfo, Boolean withHeaders)
        {
            return myList.GetClipboardText(cInfo, withHeaders, "\t");
        }
        
        public static String GetClipboardText<T>(this IEnumerable<T> myList, Boolean withHeaders)
        {
            return myList.GetClipboardText(System.Globalization.CultureInfo.InstalledUICulture, withHeaders, "\t");
        }

        public static String GetClipboardText<T>(this IEnumerable<T> myList, Boolean withHeaders, String cellSeparator)
        {
            return myList.GetClipboardText(System.Globalization.CultureInfo.InstalledUICulture, withHeaders, cellSeparator);
        }

        public static String GetClipboardText<T>(this IEnumerable<T> myList, System.Globalization.CultureInfo cInfo, Boolean withHeaders, String cellSeparator)
        {
            StringBuilder finalBuilder = new StringBuilder();
            if (withHeaders)
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    finalBuilder.AppendFormat("{0}{1}", prop.Name.GetClipboardTextFromProperty(cInfo).Replace(cellSeparator, String.Empty), cellSeparator);
                }
                if (finalBuilder.Length > cellSeparator.Length - 1)
                {
                    finalBuilder.Length -= cellSeparator.Length; //remove cell separator character
                }
                finalBuilder.Append("\r\n"); // add \r\n
            }
            foreach (var item in myList)
            {
                finalBuilder.AppendFormat("{0}\r\n", item.GetClipboardTextFromObject(cInfo, cellSeparator)); // add \r\n
            }
            if (finalBuilder.Length > 1)
            {
                finalBuilder.Length -= 2; //remove \r\n character
            }
            return finalBuilder.ToString();
        }

        public static String GetClipboardTextFromObject(this Object myObj, System.Globalization.CultureInfo cInfo, String cellSeparator)
        {
            StringBuilder finalBuilder = new StringBuilder();
            foreach (var prop in myObj.GetType().GetProperties())
            {
                finalBuilder.AppendFormat("{0}{1}", prop.GetValue(myObj, null).GetClipboardTextFromProperty(cInfo).Replace(cellSeparator, String.Empty), cellSeparator);
            }
            if (finalBuilder.Length > cellSeparator.Length - 1)
            {
                finalBuilder.Length -= cellSeparator.Length; //remove \t character
            }
            return finalBuilder.ToString();
        }
    }
}
