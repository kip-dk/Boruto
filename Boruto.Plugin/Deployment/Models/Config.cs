using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Boruto.Deployment.Extensions;

namespace Boruto.Deployment.Models
{
    [DataContract]
    public class Config
    {
        public static readonly Config Instance;

        #region static constructor of singleton config instance
        static Config()
        {
            var file = "config".CommandlineValue();
            if (!string.IsNullOrEmpty(file))
            {
                using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Open))
                {
                    Instance = InitializeFromStream(fs);
                }
            }
        }

        public static void ThrowIFMissing()
        {
            if (Instance == null)
            {
                throw new Exception($"Please provide config file as commandline parameter on form /config:filename");
            }
        }

        public bool validatePluginConfiguration()
        {
            if (this.Plugin == null)
            {
                Console.WriteLine($"Please provide plugin configuration in the config file");
                return false;
            }

            if (string.IsNullOrEmpty(this.Plugin.Package))
            {
                Console.WriteLine($"Please provide filename for plugin nuget package in the config file");
                return false;
            }

            if (string.IsNullOrEmpty(this.Plugin.Path))
            {
                Console.WriteLine($"Please provide a base path where to find the nuget package to be deployed");
                return false;
            }

            if (this.Plugin.ResolveLibraries == null || this.Plugin.ResolveLibraries.Length == 0)
            {
                Console.WriteLine($"Please provide list of libraries for plugin and service resolve");
                return false;
            }

            return true;
        }

        public bool Resolve(Assembly assm)
        {
            foreach (var rl in this.Plugin.ResolveLibraries)
            {
                if (assm.FullName.StartsWith($"{rl},"))
                {
                    return true;
                }
            }
            return false;
        }


        private static Config InitializeFromStream(System.IO.Stream str)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Config));

            return (Config)ser.ReadObject(str);
        }
        #endregion

        #region properties
        [DataMember(Name = "solution")]
        public string Solution { get; set; }

        [DataMember(Name = "plugin")]
        public PluginDefinition Plugin { get; set; }
        #endregion

        #region inner classes
        [DataContract]
        public class PluginDefinition
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }
            [DataMember(Name = "path")]
            public string Path { get; set; }

            [DataMember(Name = "package")]
            public string Package { get; set; }

            [DataMember(Name = "resolvelibraries")]
            public string[] ResolveLibraries { get; set; }
        }
        #endregion
    }
}
