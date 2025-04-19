using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.ServiceAPI
{
    internal interface IMessageService
    {
        void Inform(string message);
        void Warning(string message);
    }
}
