using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.ServiceAPI
{
    public interface IPluginDemoService
    {
        void OnCreate(Entities.bor_plugindemos.NameChanged.INameChanged target);
    }
}
