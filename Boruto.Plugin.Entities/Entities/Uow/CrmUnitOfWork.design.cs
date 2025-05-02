using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Entities
{
    public partial class CrmUnitOfWork
    {
        public IRepository<Account> Accounts => GetRepository<Account>();
    }
}
