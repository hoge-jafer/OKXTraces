using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Helper
{
    public static class LogHelper
    {
        public static void WriteLog(string message, bool state)
        {
            if (state == true)
            {
                Trace.WriteLine("OKXUI------>" + message);
            }
        }
    }
}
