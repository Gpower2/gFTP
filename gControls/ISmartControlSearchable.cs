using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gpower2.gControls
{
    public interface ISmartControlSearchable
    {
        IList Search(String argFilter);

        void OpenSubForm(Object argObject, Control argCallerControl);

        string GetTooltip(Object argObject);
    }
}
