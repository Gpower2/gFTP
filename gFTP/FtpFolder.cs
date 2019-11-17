using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtp
{
    public class FtpFolder
    {
        public String Name { get; set; }

        public DateTime Date { get; set; }

        public String FullPath { get; set; }

        public List<FtpFolder> Folders { get; set; }

        public List<FtpFile> Files { get; set; }

        public FtpFolder()
        {
            Folders = new List<FtpFolder>();
            Files = new List<FtpFile>();
        }

        public override string ToString()
        {
            return String.Format("'{0}' Folders: {1} Files: {2}", Name ?? "", Folders.Count, Files.Count);
        }
    }
}
