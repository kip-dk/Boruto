using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.Account
{
    public class AccountPlugin : BasePlugin
    {
        public void OnPreUpdate(Boruto.Plugin.Example.Entities.AccountStateChanged.IStatChanged target)
        {
            target.OnStateChanged();
        }
    }
}
