using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Boruto.Tools
{
    public class Tool
    {
        private readonly IOrganizationService orgService;
        private const string BUILDER_FILENAME = "builderSettings.json";

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

            using (var fac = new ServiceFactory(this.orgService, null, typeof(Tool).Assembly))
            {
                var meta = fac.Create<ServiceAPI.IMetadataService>();

                using (var fil = new System.IO.FileStream(BUILDER_FILENAME, System.IO.FileMode.Open))
                {
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(BuilderSettings));
                    var builderSettings = (BuilderSettings)ser.ReadObject(fil);

                    var map = new Dictionary<string, string>();

                    using (var file = new System.IO.StreamWriter($@"Uow\IUnitOfWork.design.cs"))
                    {
                        file.WriteLine($"namespace {builderSettings.Namespace}");
                        file.WriteLine("{");

                        file.WriteLine($"\tpublic partial interface IUnitOfWork");
                        file.WriteLine("\t{");
                        foreach (var entity in builderSettings.EntityNamesFilter)
                        {
                            var ent = meta.ForEntity(entity);
                            map[entity] = ent.SchemaName;
                            file.WriteLine($"\t\tIRepository<{ent.SchemaName}> {ent.SchemaName.ServiceNameOf(builderSettings.OmitEntityPrefix)} {{ get; }}");
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
                            var sn = map[entity];
                            file.WriteLine($"\t\tpublic IRepository<{sn}> {sn.ServiceNameOf(builderSettings.OmitEntityPrefix)} => GetRepository<{sn}>();");
                        }
                        file.WriteLine("\t}");

                        file.WriteLine("}");
                    }
                }
            }
        }

        [DataContract]
        internal class BuilderSettings
        {
            [DataMember(Name = "namespace")]
            public string Namespace { get; set; }

            [DataMember(Name = "entityNamesFilter")]
            public string[] EntityNamesFilter { get; set; }

            [DataMember(Name = "omitEntityPrefix")]
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
