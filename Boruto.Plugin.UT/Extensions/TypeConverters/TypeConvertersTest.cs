using Boruto.Extensions.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Extensions.TypeConverters
{
    [TestClass]
    public class TypeConvertersTest
    {
        [TestMethod]
        public void ToEnumTest()
        {
            int? number = null;
            Assert.IsNull(number.ToEnum<E1?>());

            number = 2;

            Assert.AreEqual(E1.V2, number.ToEnum<E1?>());

            F1? f1 = F1.F2;

            Assert.AreEqual(E1.V2, f1.ToEnum<E1?>());

            var os = new Microsoft.Xrm.Sdk.OptionSetValue(3);

            Assert.AreEqual(E1.V3, os.ToEnum<E1?>());

            Assert.AreEqual(E1.V3, "V3".ToEnum<E1?>());

        }

        public enum E1
        {
            V1 = 1,
            V2 = 2,
            V3 = 3
        }

        public enum F1
        {
            F1 = 1,
            F2 = 2,
            F3 = 3
        }
    }
}
