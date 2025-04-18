using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Entities
{
    public partial class bor_PlugindemoBoundmessageResponse
    {
        public string _output
        {
            set
            {
                this.Results[nameof(this.Output)] = value;
            }
        }
    }
}
