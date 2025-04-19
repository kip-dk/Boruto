using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Boruto.Deployment.Models
{
    public class NugetSpec
    {
        public NugetSpec(System.IO.Stream fs, string fileName)
        {
            var xml = new XmlDocument();
            var ns = new XmlNamespaceManager(xml.NameTable);
            ns.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");
            xml.Load(fs);

            var metaNode = xml.SelectSingleNode("/ns:package/ns:metadata", ns);
            this.Metadata = new MetadataClass
            {
                Id = metaNode.SelectSingleNode("ns:id", ns).InnerText,
                Version = metaNode.SelectSingleNode("ns:version", ns).InnerText,
                Title = metaNode.SelectSingleNode("ns:description", ns).InnerText
            };

            this.Filename = fileName;
        }

        public string Filename { get; }

        public MetadataClass Metadata { get; set; }

        public class MetadataClass
        {
            public string Id { get; set; }

            public string Version { get; set; }

            public string Title { get; set; }
        }
    }
}
