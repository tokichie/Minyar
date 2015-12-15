using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;

namespace Minyar.Charm {
    public class ItemTidset : IComparable {
        public SortedSet<string> Items { get; set; }
        public HashSet<int> Tids { get; set; }

        public ItemTidset(SortedSet<string> items, HashSet<int> ids) {
            Items = items;
            Tids = ids;
        }

        public ItemTidset(ItemTidset it) {
            Items = new SortedSet<string>(it.Items);
            Tids = new HashSet<int>(it.Tids);
        }

        public static bool operator >=(ItemTidset left, ItemTidset right) {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(ItemTidset left, ItemTidset right) {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator ==(ItemTidset left, ItemTidset right) {
            return left.ToString() == right.ToString();
        }

        public static bool operator !=(ItemTidset left, ItemTidset right) {
            return left.ToString() != right.ToString();
        }

        public int CompareTo(object obj) {
            if (obj == null) return 1;
            var right = (ItemTidset) obj;
            return Tids.Count.CompareTo(right.Tids.Count);
        }

        public override bool Equals(object obj) {
            var right = (ItemTidset) obj;
            return ToString().Equals(right.ToString());
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var item in Items.OrderBy(i => i)) {
                sb.Append(item);
            }
            sb.Append(":");
            foreach (var tid in Tids.OrderBy(tid => tid)) {
                sb.Append(tid);
            }
            return sb.ToString();
        }
    }
}
