using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Fakes
{
    public class TraceService : Microsoft.Xrm.Sdk.ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }
    }
}
