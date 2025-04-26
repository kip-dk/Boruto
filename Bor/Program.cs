using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine($"Please provide tool as first argument.");
            }

            var tool = args[0];

            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bor.Program).Assembly));

            using (var container = new CompositionContainer(aggregateCatalog))
            {

                switch (tool)
                {
                    case "deploy":
                        {
                            var configString = System.Configuration.ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
                            var connectionString = Bor.XrmOrganization.ConnectionString.ResolveConnectionStringFromStorage(configString);
                            using (var instance = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(connectionString))
                            {
                                var deployer = new Boruto.Deployment.Deployer(instance);
                                deployer.Deploy();
                            }
                            break;
                        }
                    default:
                        {
                            ICmd cmd = null;
                            try
                            {
                                cmd = container.GetExportedValue<ICmd>(args[0]);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                Console.WriteLine(ex.StackTrace);
                                var inner = ex.InnerException;

                                while (inner != null)
                                {
                                    Console.WriteLine($"  Inner: {inner.Message}");
                                    inner = inner.InnerException;
                                }
                                Console.Write("Press [Enter] to continue.");
                                Console.ReadLine();
                                return;
                            }
                            await cmd.ExecuteAsync(args.Skip(1).ToArray());
                            break;
                        }
                }
            }
        }
    }
}
