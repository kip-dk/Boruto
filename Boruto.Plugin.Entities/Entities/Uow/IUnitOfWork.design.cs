using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Entities
{
    public partial interface IUnitOfWork
    {
        IRepository<Account> Accounts { get; }
    }
}
