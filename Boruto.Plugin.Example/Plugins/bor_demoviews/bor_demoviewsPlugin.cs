using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.bor_demoviews
{
    public class bor_demoviewsPlugin : BasePlugin
    {
        public Guid OnCreate(Boruto.Plugin.Entities.bor_demoviews target, IRepository<Boruto.Plugin.Entities.bor_plugindemo> dRepo)
        {
            var clean = new Boruto.Plugin.Entities.bor_plugindemo
            {
                bor_name = target.bor_name,
                bor_number = target.bor_number
            };
            return dRepo.Create(clean);
        }

        public void OnUpdate(Boruto.Plugin.Entities.bor_demoviews target, IRepository<Boruto.Plugin.Entities.bor_plugindemo> dRepo)
        {
            var clean = new Boruto.Plugin.Entities.bor_plugindemo{ bor_plugindemoId = target.bor_demoviewsId.Value };

            if (target.Attributes.ContainsKey(nameof(target.bor_name)))
            {
                clean.bor_name = target.bor_name;
            }

            if (target.Attributes.ContainsKey(nameof(target.bor_number)))
            {
                clean.bor_number = target.bor_number;
            }
            dRepo.Update(clean);
        }

        public Boruto.Plugin.Entities.bor_demoviews OnRetrieve(Guid primaryentityid, string primaryentityname, IRepository<Boruto.Plugin.Entities.bor_plugindemo> dRepo)
        {
            var r = dRepo.Get(primaryentityid);
            return new Plugin.Entities.bor_demoviews
            {
                bor_demoviewsId = r.bor_plugindemoId.Value,
                bor_name = r.bor_name,
                bor_number = r.bor_number
            };
        }

        public Boruto.EntityCollection<Boruto.Plugin.Entities.bor_demoviews> OnRetrieveMultiple(IQueryable<Boruto.Plugin.Entities.bor_plugindemo> demoQuery)
        {
            var result = (from d in demoQuery
                          where d.statecode == Plugin.Entities.bor_plugindemo_statecode.Active
                          select d).ToArray();

            var final = new Microsoft.Xrm.Sdk.EntityCollection
            {
                MinActiveRowVersion = null,
                MoreRecords = false,
                PagingCookie = null,
                Entities = new Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Entity>(result.Length)
            };

            foreach (var r in result)
            {
                final.Entities.Add(new Boruto.Plugin.Entities.bor_demoviews
                {
                    bor_demoviewsId = r.bor_plugindemoId,
                    bor_name = r.bor_name,
                    bor_number = r.bor_number
                });
            }
            return new EntityCollection<Plugin.Entities.bor_demoviews>(final);
        }
    }
}
