using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtp
{
    public static class SizeHelper
    {
        public static String GetSize(Int64 argBytes)
        {
            // Measure scale
            String finalSize = String.Format("{0} bytes", argBytes);
            // KBytes
            double kBytes = 0;
            if(argBytes >= 1024)
            {
                kBytes = Convert.ToDouble(argBytes) / 1024.0;
                finalSize = String.Format("{0:#0.00} KB", kBytes);
            }
            // MBytes
            double MBytes = 0;
            if(kBytes >= 1024.0)
            {
                MBytes = kBytes / 1024.0;
                finalSize = String.Format("{0:#0.00} MB", MBytes);
            }
            // GBytes
            double GBytes = 0;
            if(MBytes >= 1024.0)
            {
                GBytes = MBytes / 1024.0;
                finalSize = String.Format("{0:#0.00} GB", GBytes);
            }
            // TBytes
            double TBytes = 0;
            if(GBytes >= 1024.0)
            {
                TBytes = GBytes / 1024.0;
                finalSize = String.Format("{0:#0.00} TB", TBytes);
            }
            return finalSize;
        }
    }
}
