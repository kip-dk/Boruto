using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.IMessageService))]
    public class MessageService : ServiceAPI.IMessageService
    {
        [ImportingConstructor]
        public MessageService()
        {
        }

        public void Inform(string message)
        {
            Console.WriteLine($"INFO    :{ System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }: { message }");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"WARNING :{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {message}");
        }
    }
}
