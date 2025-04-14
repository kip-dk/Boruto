using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities.bor_plugindemos
{
    public partial class bor_plugindemo : Boruto.Plugin.Entities.bor_plugindemo, bor_plugindemo.INameChanged
    {
        public interface INameChanged : Boruto.ITarget
        {
            string bor_name { get; }
            string bor_processmessage { set; }
        }
    }
}
