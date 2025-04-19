using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Models
{
    internal class Step
    {
        public int Stage { get; set; }
        public string Message { get; set; }
        public string PrimaryEntityLogicalName { get; set; }
        public int ExecutionOrder { get; set; }
        public Image TargetFilterAttributes { get; set; }
        public bool IsAsync { get; set; }
        public Image PreImage { get; set; }
        public Image PostImage { get; set; }

        public string FilteringAttributesString
        {
            get
            {
                if (this.TargetFilterAttributes == null || this.TargetFilterAttributes.AllAttributes)
                {
                    return null;
                }
                return string.Join(",", this.TargetFilterAttributes.FilteredAttributes);
            }
        }

        public string PluginRegistrationGroup => $"{this.Stage}:{this.Message}:{this.PrimaryEntityLogicalName}:{IsAsync}";
    }
}
