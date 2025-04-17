using Boruto.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.bor_plugindemo
{
    public class bor_plugindemoPlugin : BasePlugin
    {
        [Sort(1)]
        public void OnPreCreate(Entities.bor_plugindemos.NameChanged.INameChanged target, ServiceAPI.IPluginDemoService service)
        {
            service.OnCreate(target);
        }

        [Sort(2)]
        public void OnPreCreate(Entities.bor_plugindemos.NumberChanged.INumberChanged target)
        {
            target.DoStuff();
        }

        [Sort(1)]
        public void OnPreUpdate(Entities.bor_plugindemos.NameChanged.INameChanged target, ServiceAPI.IPluginDemoService service)
        {
            service.OnCreate(target);
        }

        [Sort(2)]
        public void OnPreUpdate(Entities.bor_plugindemos.NumberChanged.INumberChanged mergedimage)
        {
            mergedimage.DoStuff();
        }

        public void OnPostDelete(
            Boruto.Plugin.Example.Entities.bor_plugindemos.PreNumber.IPreNumber preimage
           ,IRepository<Boruto.Plugin.Entities.bor_plugindemo> demoRepo
            )
        {
            PluginContext.Current.Trace($"In: OnPostDelete");
            if (preimage.bor_number != null)
            {
                PluginContext.Current.Trace($"In: OnPostDelete: { preimage.bor_number }");

                var other = (from o in demoRepo.GetQuery()
                             where o.bor_number == preimage.bor_number
                               && o.bor_plugindemoId != preimage.Id
                             select o.bor_plugindemoId).FirstOrDefault();

                if (other != null)
                {
                    PluginContext.Current.Trace($"In: OnPreDelete: {other}");
                    var clean = new Boruto.Plugin.Entities.bor_plugindemo { bor_plugindemoId = other.Value };
                    clean.bor_changelog = $"delete: {preimage.Id}: { preimage.bor_number }";
                    demoRepo.Update(clean);
                }
            }
        }
    }
}
