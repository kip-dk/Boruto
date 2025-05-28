using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Boruto.Tools
{
    public class Tool
    {
        private readonly IOrganizationService orgService;

        private const string BUILDER_FILENAME = "builderSettings.json";

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public Tool(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public void GenerateIUnitOfWork()
        {

            if (!System.IO.File.Exists(BUILDER_FILENAME))
            {
                Console.WriteLine($"No file  [{ BUILDER_FILENAME }] was found in this directory");
                return;
            }

            using (var fil = new System.IO.FileStream(BUILDER_FILENAME, System.IO.FileMode.Open))
            {
                var builderSettings = System.Text.Json.JsonSerializer.Deserialize<BuilderSettings>(fil, jsonOptions);

                using (var file = new System.IO.StreamWriter($@"Uow\IUnitOfWork.design.cs"))
                {
                    file.WriteLine($"namespace { builderSettings.Namespace }");
                    file.WriteLine("{");

                    file.WriteLine($"\tpublic partial interface IUnitOfWork");
                    file.WriteLine("\t{");
                    foreach (var entity in builderSettings.EntityNamesFilter)
                    {
                        file.WriteLine($"\t\tIRepository<{entity}> {entity.ServiceNameOf(builderSettings.OmitEntityPrefix)} {{ get; }}");
                    }
                    file.WriteLine("\t}");

                    file.WriteLine("}");
                }

                using (var file = new System.IO.StreamWriter($@"Uow\CrmUnitOfWork.design.cs"))
                {
                    file.WriteLine($"namespace {builderSettings.Namespace}");
                    file.WriteLine("{");

                    file.WriteLine($"\tpublic partial class CrmUnitOfWork");
                    file.WriteLine("\t{");
                    foreach (var entity in builderSettings.EntityNamesFilter)
                    {
                        file.WriteLine($"\t\tpublic IRepository<{entity}> {entity.ServiceNameOf(builderSettings.OmitEntityPrefix)} => GetRepository<{entity}>();");
                    }
                    file.WriteLine("\t}");

                    file.WriteLine("}");
                }
            }
        }

        internal class BuilderSettings
        {
            public string Namespace { get; set; }
            public string[] EntityNamesFilter { get; set; }
            public string[] OmitEntityPrefix { get; set; }
        }
    }

    internal static class ToolsLocalExtensions
    {
        internal static string ServiceNameOf(this string name, string[] ommitPrefix)
        {
            if (ommitPrefix != null && ommitPrefix.Length > 0) 
            {
                foreach (var pre in ommitPrefix)
                {
                    if (name.StartsWith(pre))
                    {
                        name = name.Substring(pre.Length);
                    }
                }
            }
            return name.Substring(0, 1).ToUpper() + name.Substring(1) + "s";
        }
    }
}
