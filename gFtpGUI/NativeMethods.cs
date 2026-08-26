using System;
using System.Runtime.InteropServices;

namespace gFtpGUI
{
    /// <summary>
    /// Native Windows API methods for theming support.
    /// Based on the gMKVExtractGUI NativeMethods reference implementation.
    /// </summary>
    public static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

        public enum DWMWINDOWATTRIBUTE : int
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20, // For Windows 10 18985+ and Windows 11
            DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1 = 19, // For Windows 10 17763 to 18985
        }

        /// <summary>
        /// Helper method to attempt setting the dark mode for a window's title bar and non-client area.
        /// </summary>
        /// <param name="handle">The window handle.</param>
        /// <param name="enabled">Whether dark mode should be enabled.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool TrySetImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            try
            {
                int darkModeValue = enabled ? 1 : 0;
                // First, try with attribute 20 (DWMWA_USE_IMMERSIVE_DARK_MODE)
                int result = DwmSetWindowAttribute(
                    handle,
                    DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                    ref darkModeValue,
                    Marshal.SizeOf(typeof(int)));

                // if the above fails, try the older attribute value (19)
                if (result != 0) // 0 means S_OK (success)
                {
                    result = DwmSetWindowAttribute(
                        handle,
                        DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1,
                        ref darkModeValue,
                        Marshal.SizeOf(typeof(int)));
                }

                return result == 0;
            }
            catch (Exception)
            {
                // This can happen if dwmapi.dll is not found or the attribute is not supported.
                return false;
            }
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        /// <summary>
        /// Sets the window theme to dark mode or light mode based on the provided boolean value.
        /// This themes scrollbars, buttons, and other common controls.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="darkModeEnabled">Whether dark mode should be enabled.</param>
        /// <param name="pszSubIdList">Optional sub-ID list.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SetWindowThemeManaged(IntPtr hWnd, bool darkModeEnabled, string pszSubIdList = null)
        {
            string mode = darkModeEnabled ? "DarkMode_Explorer" : "ClearMode_Explorer";
            return SetWindowThemeInternal(hWnd, mode, pszSubIdList);
        }

        /// <summary>
        /// Sets the window theme specifically for ComboBox controls.
        /// ComboBox controls use a different theme class than other controls.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="darkModeEnabled">Whether dark mode should be enabled.</param>
        /// <param name="pszSubIdList">Optional sub-ID list.</param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SetWindowThemeForComboBoxManaged(IntPtr hWnd, bool darkModeEnabled, string pszSubIdList = null)
        {
            string mode = darkModeEnabled ? "DarkMode_CFD" : "ClearMode_CFD";
            return SetWindowThemeInternal(hWnd, mode, pszSubIdList);
        }

        private static bool SetWindowThemeInternal(IntPtr hWnd, string mode, string pszSubIdList = null)
        {
            try
            {
                int result = SetWindowTheme(hWnd, mode, pszSubIdList);
                return result == 0;
            }
            catch (Exception)
            {
                // uxtheme.dll may not be available or the theme may not be supported
                return false;
            }
        }
    }
}
