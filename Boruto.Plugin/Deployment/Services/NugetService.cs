using Boruto.Deployment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.INugetService))]
    internal class NugetService : ServiceAPI.INugetService
    {
        private Models.Config config;
        private Models.NugetSpec spec;
        private DLLCode[] dlls;

        [ImportingConstructor]
        public NugetService()
        {
            this.config = Models.Config.Instance;
        }

        public Models.NugetSpec GetSpec()
        {
            this.initialize();
            return this.spec;
        }

        public DLLCode[] GetLibNet64()
        {
            this.initialize();
            return this.dlls;
        }

        private string _packageFile;
        private string PackageFile()
        {
            var dirInfo = new System.IO.DirectoryInfo(this.config.Plugin.Path);
            var file = dirInfo.GetFiles().Where(r => r.FullName.EndsWith(".nupkg") && r.Name.StartsWith($"{this.config.Plugin.Package}.")).OrderByDescending(r => r.CreationTime).Select(r => r.FullName).FirstOrDefault();
            if (string.IsNullOrEmpty(file))
            {
                throw new System.IO.FileNotFoundException($"No files like: {this.config.Plugin.Path}{ this.config.Plugin.Package }.$version.nupkg not found"); 
            }
            return file;
        }
        private void initialize()
        {
            if (spec != null)
            {
                return;
            }

            var file = this.PackageFile();
            Console.WriteLine($"Found package: { file }");

            using (ZipArchive zip = ZipFile.OpenRead(file))
            {
                var result = new List<Models.DLLCode>();

                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName == $"{this.config.Plugin.Package}.nuspec")
                    {
                        using (var ent = entry.Open())
                        {
                            this.spec = new NugetSpec(ent, file);
                            continue;
                        }
                    }

                    if (entry.FullName.StartsWith("lib/net462/") && entry.Name != "Boruto.Plugin.dll")
                    {
                        var next = new DLLCode
                        {
                            Name = entry.Name
                        };
                        using (var ent = entry.Open())
                        {
                            using (var mem = new System.IO.MemoryStream())
                            {
                                ent.CopyTo(mem);
                                next.Code = mem.ToArray();
                            }
                            result.Add(next);
                        }
                    }
                }
                this.dlls = result.ToArray();
            }

            if (spec == null || dlls == null || dlls.Length == 0)
            {
                throw new Exception($"Nuget package for deploy could not be resolved");
            }
        }
    }
}
