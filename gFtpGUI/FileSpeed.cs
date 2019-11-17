using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtpGUI
{
   public class FileSpeed : IComparable
    {
        public Int64? Size { get; set; }
        public Double? TotalMilliseconds { get; set; }

        public double Speed
        {
            get
            {
                if (Size != null && TotalMilliseconds != null)
                {
                    return (Convert.ToDouble(Size) / 1024.0) / (TotalMilliseconds.Value / 1000.0);
                }
                else
                {
                    return 0;
                }
            }
        }

        public override string ToString()
        {
            if (Size != null && TotalMilliseconds != null)
            {
                return String.Format("{0:#0.00} KB/s", Speed);
            }
            else
            {
                return "";
            }
        }

        public int CompareTo(object obj)
        {
            return Speed.CompareTo((obj as FileSpeed).Speed);
        }
    }
}
