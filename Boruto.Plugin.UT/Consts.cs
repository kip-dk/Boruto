using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT
{
    public static class Consts
    {
        public static readonly Assembly[] Assemblies = new Assembly[]
        {
            typeof(Boruto.Plugin.Entities.Account).Assembly,
            typeof(Boruto.Plugin.Example.Entities.AccountStateChanged).Assembly,
            typeof(Boruto.Plugin.UT.Extensions.Reflection.ReflectionMethodsTest).Assembly
        };

    }
}
