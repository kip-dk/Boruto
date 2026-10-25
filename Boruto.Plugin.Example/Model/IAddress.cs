using Boruto.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Model
{
    public interface IAddress : IMerged
    {
        [TargetFilter]
        string Line1 { get; set; }
        [TargetFilter]
        string City { get; set; }

        string Country { get; set; }
    }
}
