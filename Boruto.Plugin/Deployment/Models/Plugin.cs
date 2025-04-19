using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Models
{
    internal class Plugin
    {
        private List<Step> steps = new List<Step>();

        public Plugin(Type type, Step[] steps, bool virtualEntity = false)
        {
            this.Type = type;
            this.IsVirtualEntityPlugin = virtualEntity;
            this.steps.AddRange(steps);
        }

        public bool IsVirtualEntityPlugin { get; private set; }
        public Type Type { get; private set; }
        public Step[] Steps => this.steps.ToArray();

        public Entities.PluginType CurrentCrmInstance { get; set; }

        private string StageName(int stage, bool async)
        {
            switch (stage)
            {
                case 10: return "validate";
                case 20: return "pre";
                case 30: return "virtual";
                case 40: return async ? "post async" : "post";
            }

            throw new NotImplementedException($"Stage  {stage} is not implemented");
        }

        public string NameOf(int stage, string message, bool async, string logicalname)
        {
            var of = logicalname != null ? $" of {logicalname}" : "";
            return $"{this.Type.FullName} {StageName(stage, async)} {message}{of}";
        }

    }
}
