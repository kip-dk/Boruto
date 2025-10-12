using Boruto.Extensions.Generics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Extensions.Generics
{
    [TestClass]
    public class GenericsMethodsTest
    {
        [TestMethod]
        public void OrderChildrenFirst() 
        {
            var items = new List<Node>
            {
                new Node { Id = 1, ParentId = null },
                new Node { Id = 2, ParentId = 1 },
                new Node { Id = 3, ParentId = 1 },
                new Node { Id = 4, ParentId = 2 },
                new Node { Id = 5, ParentId = 2 }
            };

            var orders = items.OrderChildrenFirst(r => r.Id, r => r.ParentId);

            var orderString = string.Join(", ", orders.Select(r => r.Id));
            Assert.AreEqual("4, 5, 2, 3, 1", orderString);
        }

        class Node
        {
            public int Id { get; set; }
            public int? ParentId { get; set; } // null = root
        }
    }
}
