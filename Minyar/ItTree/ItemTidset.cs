using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;

namespace Minyar.ItTree {
    public class ItemTidset : IComparable {
        public SortedSet<string> TransactionItems { get; set; }
        public HashSet<int> TransactionIds { get; set; }
        public int SupportCount { get; set; }

        public ItemTidset(SortedSet<string> items, HashSet<int> ids) {
            TransactionItems = items;
            TransactionIds = ids;
            SupportCount = 0;
        }

        public static bool operator >=(ItemTidset left, ItemTidset right) {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(ItemTidset left, ItemTidset right) {
            return left.CompareTo(right) < 0;
        }

        public int CompareTo(object obj) {
            if (obj == null) return 1;
            var right = (ItemTidset) obj;
            return ToString().CompareTo(right.ToString());
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var item in TransactionItems) {
                sb.Append(item);
            }
            return sb.ToString();
        }
    }
}
