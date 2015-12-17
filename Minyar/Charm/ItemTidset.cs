using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;

namespace Minyar.Charm {
    public class ItemTidSet<T1, T2> : IComparable {
        private SortedSet<T1> items; 
        private HashSet<T2> tids;
        public int SupportCount { get; private set; }
        public int ItemCount { get; private set; }

        public string ItemsString {
            get { return string.Join(", ", items); }
        }

        public SortedSet<T1> Items {
            get { return items; }
            set {
                items = value;
                ItemCount = value.Count;
            }
        }

        public HashSet<T2> Tids {
            get { return tids; }
            set {
                tids = value;
                SupportCount = value.Count;
            }
        }

        public ItemTidSet(IEnumerable<T1> items, IEnumerable<T2> ids) {
            Items = new SortedSet<T1>(items);
            Tids = new HashSet<T2>(ids);
        }

        public ItemTidSet(ItemTidSet<T1, T2> it) {
            Items = new SortedSet<T1>(it.Items);
            Tids = new HashSet<T2>(it.Tids);
        }

        public int GetFrequency() {
            if (typeof (T2) != typeof (RepeatableTid)) return Tids.Count;
            var tids = Tids as HashSet<RepeatableTid>;
            return tids.Min(i => i.Occurrences);
        }

        public static bool operator >=(ItemTidSet<T1, T2> left, ItemTidSet<T1, T2> right) {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(ItemTidSet<T1, T2> left, ItemTidSet<T1, T2> right) {
            return left.CompareTo(right) <= 0;
        }

        public int CompareTo(object obj) {
            if (obj == null) return 1;
            var right = (ItemTidSet<T1, T2>) obj;
            return GetFrequency().CompareTo(right.GetFrequency());
        }

        public override bool Equals(object obj) {
            var right = obj as ItemTidSet<T1, T2>;
            if (right == null) return false;
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
