using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gpower2.gControls
{
    public static class ExceptionExtensions
    {
        public static void ShowException(this Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show("Προέκυψε σφάλμα!\r\n\r\n" + ex.Message, "Προέκυψε σφάλμα!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
