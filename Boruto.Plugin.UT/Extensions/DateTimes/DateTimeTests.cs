using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boruto.Extensions.DateTimes;

namespace Boruto.Plugin.UT.Extensions.DateTimes
{
    [TestClass]
    public class DateTimeTests
    {
        [TestMethod]
        public void StartOfWeekTest()
        {
            var monday = new DateTime(2026, 1, 12, 0, 0, 0, 0,DateTimeKind.Utc);
            var sunday = new DateTime(2026, 1, 18, 23, 59, 59, 0, DateTimeKind.Utc);

            var date = new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc);

            var endofweek = date.EndOfWeek();

            Assert.AreEqual(monday, date.StartOfWeek());
            Assert.AreEqual(sunday, endofweek);



        }
    }
}
