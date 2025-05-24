using Microsoft.Xrm.Sdk;

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

            using (var fac = new Boruto.ServiceFactory(this.orgService, null, typeof(Boruto.BasePlugin).Assembly))
            {
                var dpService = fac.Create<ServiceAPI.IDeployService>();
                dpService.Deploy();
            }
        }
    }
}
