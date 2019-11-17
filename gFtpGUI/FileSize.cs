using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtpGUI
{
    public class FileSize : IComparable
    {
        public Int64 Size { get; set; }

        public FileSize(Int64 argSize) { Size = argSize; }

        public override string ToString()
        {
            return gFtp.SizeHelper.GetSize(Size);
        }

        public int CompareTo(object obj)
        {
            return Size.CompareTo((obj as FileSize).Size);
        }

        public static FileSize operator +(FileSize fs1, FileSize fs2)
        {
            return new gFtpGUI.FileSize(fs1.Size + fs2.Size);
        }

        public static FileSize operator -(FileSize fs1, FileSize fs2)
        {
            return new gFtpGUI.FileSize(fs1.Size - fs2.Size);
        }

        public static FileSize operator *(FileSize fs1, FileSize fs2)
        {
            return new gFtpGUI.FileSize(fs1.Size * fs2.Size);
        }

        public static FileSize operator /(FileSize fs1, FileSize fs2)
        {
            return new gFtpGUI.FileSize(fs1.Size / fs2.Size);
        }
    }
}
