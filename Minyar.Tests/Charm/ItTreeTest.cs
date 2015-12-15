using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Charm;
using NUnit.Framework;

namespace Minyar.Tests.Charm {
    [TestFixture]
    class ItTreeTest {
        [Test]
        public void TestCharmOnPaperExample() {
            var a = new ItemTidset(new SortedSet<string>(new [] {"a"}), new HashSet<int>(new [] {1, 3, 4, 5}));
            var c = new ItemTidset(new SortedSet<string>(new [] {"c"}), new HashSet<int>(new [] {1, 2, 3, 4, 5, 6}));
            var d = new ItemTidset(new SortedSet<string>(new [] {"d"}), new HashSet<int>(new [] {2, 4, 5, 6}));
            var t = new ItemTidset(new SortedSet<string>(new [] {"t"}), new HashSet<int>(new [] {1, 3, 5, 6}));
            var w = new ItemTidset(new SortedSet<string>(new [] {"w"}), new HashSet<int>(new [] {1, 2, 3, 4, 5}));
            var transactions = new List<ItemTidset>(new [] {a, c, d, w, t});
            var it = new ItTree(transactions, 3);
            var res = it.GetClosedItemSets();
            var acw = new ItemTidset(new SortedSet<string>(new [] {"a", "c", "w"}), new HashSet<int>(new [] {1, 3, 4, 5}));
            var actw = new ItemTidset(new SortedSet<string>(new [] {"a", "c", "t", "w"}), new HashSet<int>(new [] {1, 3, 5}));
            var cd = new ItemTidset(new SortedSet<string>(new [] {"c", "d"}), new HashSet<int>(new [] {2, 4, 5, 6}));
            var cdw = new ItemTidset(new SortedSet<string>(new [] {"c", "d", "w"}), new HashSet<int>(new [] {2, 4, 5}));
            var ct = new ItemTidset(new SortedSet<string>(new [] {"c", "t"}), new HashSet<int>(new [] {1, 3, 5, 6}));
            var cw = new ItemTidset(new SortedSet<string>(new [] {"c", "w"}), new HashSet<int>(new [] {1, 2, 3, 4, 5}));
            var ans = new List<ItemTidset>(new [] {c, cw, ct, cd, cdw, acw, actw});
            Assert.That(new HashSet<ItemTidset>(res).SetEquals(ans));
        }
    }
}
