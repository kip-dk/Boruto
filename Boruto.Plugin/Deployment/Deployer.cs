using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Boruto.Deployment
{
    public class Deployer
    {
        private readonly IOrganizationService orgService;

        public Deployer(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public void Deploy()
        {
            // System.Console.WriteLine($"attach debugger now");
            // System.Console.ReadLine();

            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Boruto.BasePlugin).Assembly));


            using (var container = new CompositionContainer(aggregateCatalog))
            {
                container.ComposeExportedValue<Microsoft.Xrm.Sdk.IOrganizationService>(this.orgService);
                var dpService = container.GetExportedValue<ServiceAPI.IDeployService>();
                dpService.Deploy();
            }
        }

        public void ListComponentTypesFor(string solutionName)
        {
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Boruto.BasePlugin).Assembly));


            using (var container = new CompositionContainer(aggregateCatalog))
            {
                container.ComposeExportedValue<Microsoft.Xrm.Sdk.IOrganizationService>(this.orgService);
                var uow = container.GetExportedValue<Entities.IUnitOfWork>();
                var sol = (from s in uow.Solutions.GetQuery()
                           where s.UniqueName == solutionName
                           select s).Single();

                Console.WriteLine($"Found: { sol.UniqueName }");

                var types = (from c in uow.SolutionComponents.GetQuery()
                             where c.SolutionId.Id == sol.SolutionId.Value
                             select c).ToArray();

                foreach (var type in types)
                {
                    Console.WriteLine($"Fandt: { type.ComponentTypeName } { type.ComponentType }");
                }
            }
        }
    }
}
