using gFtp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace gFtpGUI
{
    public class Job : IComparable
    {
        public String AriaPath { get; set; }
        public String FtpPath { get; set; }
        public String FtpFilename { get; set; }
        public String LocalPath { get; set; }
        public String LocalFilename { get; set; }
        public FileSize Size { get; set; }
        public gFTP Ftp { get; set; }

        public override string ToString()
        {
            return String.Format("{0} => {1}", UrlHelper.Combine(FtpPath, FtpFilename), Path.Combine(LocalPath, LocalFilename));
        }

        public int CompareTo(object obj)
        {
            return FtpFilename.CompareTo((obj as Job).FtpFilename);
        }
    }
}
