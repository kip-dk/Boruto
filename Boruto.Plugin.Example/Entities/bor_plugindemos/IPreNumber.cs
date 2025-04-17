using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities.bor_plugindemos
{
    public class PreNumber : Boruto.Plugin.Entities.bor_plugindemo, PreNumber.IPreNumber
    {
        public interface IPreNumber : Boruto.IPreImage
        {
            int? bor_number { get; }
        }
    }
}
