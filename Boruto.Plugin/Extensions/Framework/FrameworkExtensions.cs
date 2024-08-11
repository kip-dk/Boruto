using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Extensions.Framework
{
    public static class FrameworkExtensions
    {
        private static readonly string[] EntityMessages = new string[]
        {
            "Create",
            "Update",
            "Delete",
            "Retrieve",
            "RetrieveMultiple"
        };

        public static string ToMethodName(this int stage, string message, int mode) 
        {
            var sb = new StringBuilder("On");
            switch (stage)
            {
                case 10: 
                    sb.Append("Validate");
                    break;
                case 20:
                    sb.Append("Pre");
                    break;
                case 40:
                    sb.Append("Post");
                    break;
            }

            if (EntityMessages.Contains(message))
            {
                sb.Append(message);
            }

            if (mode > 0)
            {
                sb.Append("Async");
            }

            return sb.ToString();
        }
    }
}
