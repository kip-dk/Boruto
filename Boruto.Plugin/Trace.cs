using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public static class Trace
    {
        public static void Info(string message)
        {
            var cur = PluginContext.Current;
            if (cur != null)
            {
                cur.Trace($"INFO Utc : { System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }: { message }");
            } else
            {
                Console.WriteLine($"INFO Utc : {System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: {message}");
            }
        }

        public static void Warning(string message)
        {
            var cur = PluginContext.Current;
            if (cur != null)
            {
                cur.Trace($"WARN Utc : {System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: {message}");
            }
            else
            {
                Console.WriteLine($"WARN Utc : {System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: {message}");
            }
        }

        public static void Error(string message)
        {
            var cur = PluginContext.Current;
            if (cur != null)
            {
                cur.Trace($"ERROR Utc: {System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: {message}");
            }
            else
            {
                Console.WriteLine($"ERROR Utc: {System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: {message}");
            }
        }
    }
}
