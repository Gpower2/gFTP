using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace gFtpGUI
{
    public static class gWindowsDarkMode
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static bool UseImmersiveDarkMode(this Form form, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                int attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(form.Handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            if (Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemPlatform != Microsoft.DotNet.PlatformAbstractions.Platform.Windows)
            {
                return false;
            }

            if (!Version.TryParse(Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemVersion, out Version osVersion))
            {
                return false;
            }

            if (osVersion.Major < 10)
            {
                return false;
            }

            return osVersion.Build >= build;
        }
    }
}
