using Boruto.Attributes;
using Boruto.Extensions.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities.bor_plugindemos
{
    public class NumberChanged : Boruto.Plugin.Entities.bor_plugindemo, NumberChanged.INumberChanged 
    {
        int? INumberChanged.Pre_bor_number => this.Attributes.PreValueOf<int?>(nameof(bor_number));

        void INumberChanged.DoStuff()
        {
            var me = this as NumberChanged.INumberChanged;
            var start = this.bor_changelog != null ? $"{ this.bor_changelog }\n" : "";
            this.bor_changelog = $"{ start }{ (me.Pre_bor_number != null ? me.Pre_bor_number.ToString() : "-") } -> {(me.bor_number != null ? me.bor_number.ToString() : "-") }";
        }

        public interface INumberChanged : IMerged
        {
            [TargetFilter]
            int? bor_number { get; }

            string bor_changelog { get; set; }
            int? Pre_bor_number { get; }

            void DoStuff();
        }
    }
}
