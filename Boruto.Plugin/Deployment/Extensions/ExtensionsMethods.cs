using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Extensions
{
    internal static class ExtensionsMethods
    {
        public static string CommandlineValue(this string name)
        {
            var n = $"/{name}:";
            var v = System.Environment.GetCommandLineArgs().Where(r => r.StartsWith(n)).SingleOrDefault();
            if (!string.IsNullOrEmpty(v))
            {
                return v.Substring(n.Length);
            }
            return null;
        }
    }
}
