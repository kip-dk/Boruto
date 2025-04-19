namespace Boruto
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var instance = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(args[0]))
            {
                var deployer = new Boruto.Deployment.Deployer(instance);
                deployer.Deploy();
            }
        }
    }
}
