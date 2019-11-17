using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gFtp
{
    public static class UrlHelper
    {
        public static String Combine(params String[] argUrls)
        {
            String finalUrl = argUrls[0];
            for (int i = 1; i < argUrls.Length; i++)
            {
                finalUrl = String.Format("{0}{1}",
                    finalUrl.EndsWith("/") ? finalUrl : String.Format("{0}/", finalUrl), argUrls[i]);                
            }
            return finalUrl;
        }
    }
}
