using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bor.Extensions.Strings
{
    public static class StringsExtensions
    {
        public static string GetParameter(this string url, string name, char sep = ';')
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            var spl = url.Split(sep).Select(r => r.Replace(" ", ""));
            var match = (from s in spl where s.ToLower().StartsWith(name.ToLower() + "=") select s).FirstOrDefault();
            if (!string.IsNullOrEmpty(match))
            {
                var index = match.IndexOf("=");
                return match.Substring(index + 1).Trim();
            }
            return null;
        }

        public static string GetCommandlineParameter(this string name)
        {
            var pName = $"/{name}:";
            var pms = System.Environment.GetCommandLineArgs().Where(r => r.StartsWith(pName)).FirstOrDefault();
            if (pms != null)
            {
                return pms.Substring(pName.Length).Trim();
            }
            return null;
        }

        public static bool SpaceOnly(this string[] lines)
        {
            if (lines == null || lines.Length == 0)
            {
                return true;
            }

            foreach (var lin in lines)
            {
                var trim = lin.Trim();
                if (!string.IsNullOrEmpty(trim))
                {
                    return false;
                }
            }
            return true;
        }

        public static readonly string REPLACE = "!\"#¤%&/()=?{[]}|'";

        public static string ToCSharpName(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var result = string.Empty;
            var first = true;
            foreach (var ch in value)
            {
                if (first && !char.IsLetter(ch) && ch != '_')
                {
                    result += "_";
                }

                first = false;

                if (Char.IsLetterOrDigit(ch))
                {
                    result += ch;
                }
                else
                {
                    result += "_";
                }
            }
            return result;
        }

        public static string ToJsonString<T>(this T data)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

            using (var mem = new System.IO.MemoryStream())
            {
                ser.WriteObject(mem, data);

                return System.Text.Encoding.UTF8.GetString(mem.ToArray());
            }
        }
    }
}
