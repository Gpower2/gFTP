using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtp
{
    public class FtpFile
    {
        public String Name { get; set; }

        public Int64 Size { get; set; }

        public DateTime Date { get; set; }

        public String FullPath { get; set; }

        public FtpFolder Folder { get; set; }

        public override string ToString()
        {
            return String.Format("'{0}' {1} {2} bytes", Name ?? "", Date.ToString("MMM dd yyyy HH:mm"), SizeHelper.GetSize(Size));
        }

        public String Extension()
        {
            if (!String.IsNullOrWhiteSpace(Name))
            {
                if (Name.Contains("."))
                {
                    return Name.Substring(Name.LastIndexOf("."));
                }
                else
                {
                    return "";
                }
            }
            return "";
        }
    }
}
