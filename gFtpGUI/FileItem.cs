using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace gFtpGUI
{
    public class FileItem
    {
        public Bitmap Icon { get; set; }

        public String Name { get; set; }

        public FileSize Size { get; set; }

        public DateTime Date { get; set; }

        public String Type { get; set; }
    }
}
