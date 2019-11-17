using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtp
{
    public static class StringHelper
    {
        public static String RemoveDoubleSpaces(this String str)
        {
            while(str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }
    }
}
