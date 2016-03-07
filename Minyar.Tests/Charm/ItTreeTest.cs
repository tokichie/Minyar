using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minyar.Charm;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Minyar.Tests.Charm {
    [TestFixture]
    class ItTreeTest {
        [Test]
        public void TestCharmOnPaperExample() {
            var a = new ItemTidSet<string, int>(new [] {"a"}, new [] {1, 3, 4, 5});
            var c = new ItemTidSet<string, int>(new [] {"c"}, new [] {1, 2, 3, 4, 5, 6});
            var d = new ItemTidSet<string, int>(new [] {"d"}, new [] {2, 4, 5, 6});
            var t = new ItemTidSet<string, int>(new [] {"t"}, new [] {1, 3, 5, 6});
            var w = new ItemTidSet<string, int>(new [] {"w"}, new [] {1, 2, 3, 4, 5});
            var transactions = new List<ItemTidSet<string, int>>(new [] {a, c, d, w, t});
            var it = new CharmItTree(transactions, 3);
            var res = it.GetClosedItemSets();
            var acw = new ItemTidSet<string, int>(new [] {"a", "c", "w"}, new [] {1, 3, 4, 5});
            var actw = new ItemTidSet<string, int>(new [] {"a", "c", "t", "w"}, new [] {1, 3, 5});
            var cd = new ItemTidSet<string, int>(new [] {"c", "d"}, new [] {2, 4, 5, 6});
            var cdw = new ItemTidSet<string, int>(new [] {"c", "d", "w"}, new [] {2, 4, 5});
            var ct = new ItemTidSet<string, int>(new [] {"c", "t"}, new [] {1, 3, 5, 6});
            var cw = new ItemTidSet<string, int>(new [] {"c", "w"}, new [] {1, 2, 3, 4, 5});
            var ans = new List<ItemTidSet<string, int>>(new [] {c, cw, ct, cd, cdw, acw, actw});
            Assert.That(new HashSet<ItemTidSet<string, int>>(res).SetEquals(ans));
        }

        [Test]
        public void TestCharmOnRepeatableItem() {
            var a = new ItemTidSet<string, RepeatableTid>(new [] {"a"}, new [] {new RepeatableTid(1, 1)});
            var b = new ItemTidSet<string, RepeatableTid>(new [] {"b"}, new [] {new RepeatableTid(1, 1)});
            var c = new ItemTidSet<string, RepeatableTid>(new [] {"c"},
                new [] {new RepeatableTid(1, 3), new RepeatableTid(2, 4), new RepeatableTid(3, 3)});
            var d = new ItemTidSet<string, RepeatableTid>(new [] {"d"},
                new [] {new RepeatableTid(1, 1), new RepeatableTid(2, 1), new RepeatableTid(3, 1)});
            var e = new ItemTidSet<string, RepeatableTid>(new [] {"e"},
                new [] {new RepeatableTid(1, 1), new RepeatableTid(2, 1), new RepeatableTid(3, 1)});
            var f = new ItemTidSet<string, RepeatableTid>(new [] {"f"},
                new [] {new RepeatableTid(1, 1), new RepeatableTid(2, 1), new RepeatableTid(3, 1)});
            var g = new ItemTidSet<string, RepeatableTid>(new [] {"g"},
                new [] {new RepeatableTid(2, 1), new RepeatableTid(3, 2)});
            var transactions = new List<ItemTidSet<string, RepeatableTid>>(new[] { a, b, c, d, e, f, g });
            var it = new ItTree(transactions, 2, 2, 2);
            var res = it.GetClosedItemSets();
            var cg = new ItemTidSet<string, RepeatableTid>(new [] {"c", "g"},
                new [] {new RepeatableTid(2, 1), new RepeatableTid(3, 2)});
            var cdef = new ItemTidSet<string, RepeatableTid>(new [] {"c", "d", "e", "f"},
                new [] {new RepeatableTid(1, 1), new RepeatableTid(2, 1), new RepeatableTid(3, 1)});
            var cdefg = new ItemTidSet<string, RepeatableTid>(new [] {"c", "d", "e", "f", "g"},
                new [] {new RepeatableTid(2, 1), new RepeatableTid(3, 1)});
            var ans = new List<ItemTidSet<string, RepeatableTid>>(new [] {c, cg, cdef, cdefg});
            Assert.That(new HashSet<ItemTidSet<string, RepeatableTid>>(res).SetEquals(ans));
        }

        [Test]
        public void TestIntersect() {
            var it = new ItTree(null, 0, 0, 0);
            var obj = new PrivateObject(it);
            var c = new[] { new RepeatableTid(1, 3), new RepeatableTid(2, 4), new RepeatableTid(3, 3) };
            var d = new[] { new RepeatableTid(1, 1), new RepeatableTid(2, 3), new RepeatableTid(3, 2) };
            var res = obj.Invoke("IntersectTid", new [] { new HashSet<RepeatableTid>(c), new HashSet<RepeatableTid>(d)});
        }
    }
}
