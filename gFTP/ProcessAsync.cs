using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gFtp
{
    public static class ProcessAsync
    {
        public static Task<Process> StartAsync(string filename)
        {
            return Task.Run(() => Process.Start(filename));
        }

        public static Task<Process> StartAsync(string filename, string arguments)
        {
            return Task.Run(() => Process.Start(filename, arguments));
        }
    }
}
