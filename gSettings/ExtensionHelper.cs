using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gpower2.gSettings
{
    public static class ExtensionHelper
    {
        public static Object IsNull(this Object argObject, Object argDefaultValue)
        {
            return argObject == null ? argDefaultValue : argObject;
        }
    }
}
