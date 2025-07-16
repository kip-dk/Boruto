using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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
                Console.WriteLine($"No file  [{BUILDER_FILENAME}] was found in this directory");
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
                        var dubs = builderSettings.Dubs();
                        foreach (var entity in builderSettings.EntityNamesFilter)
                        {
                            var ent = meta.ForEntity(entity);
                            map[entity] = ent.SchemaName;
                            file.WriteLine($"\t\tIRepository<{ent.SchemaName}> {ent.SchemaName.ServiceNameOf(builderSettings.OmitEntityPrefix, dubs)} {{ get; }}");
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
                            file.WriteLine($"\t\tpublic IRepository<{sn}> {sn.ServiceNameOf(builderSettings.OmitEntityPrefix, null)} => GetRepository<{sn}>();");
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

            internal string[] Dubs()
            {
                var all = (from s in EntityNamesFilter
                           select s.ServiceNameOf(this.OmitEntityPrefix, null)).ToArray();

                return (from a in all
                        group a by a into grp
                        select new
                        {
                            grp.Key,
                            grp.ToArray().Length
                        }).Where(r => r.Length > 1).Select(r => r.Key).ToArray();
            }
        }
    }

    internal static class ToolsLocalExtensions
    {
        internal static string ServiceNameOf(this string name, string[] ommitPrefix, string[] dubs)
        {
            var finalName = name;
            if (ommitPrefix != null && ommitPrefix.Length > 0)
            {
                foreach (var pre in ommitPrefix)
                {
                    if (finalName.StartsWith(pre))
                    {
                        finalName = finalName.Substring(pre.Length);
                    }
                }
            }

            var result = finalName.Substring(0, 1).ToUpper() + finalName.Substring(1) + "s";

            if (dubs != null && dubs.Contains(result))
            {
                return name.Substring(0, 1).ToUpper() + name.Substring(1) + "s";
            }

            return result;
        }
    }
}
