using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAO;

namespace FP.DAL.DAO {
    public class JsonItemSet {
        private List<JsonItem> items;

        public int SupportCount;

        public JsonItemSet() {
            items = new List<JsonItem>();
            SupportCount = -1;
        }

        public void AddItem(JsonItem item) {
            items.Add(item);
            SupportCount = -1;
        }
        //remove item
        public JsonItem GetItem(int position) {
            if (position < items.Count)
                return items[position];
            else
                return null;
        }
        //add item into item set
        public bool IsEmpty() {
            return items.Count == 0;
        }
        //add item into item set
        public int GetLength() {
            return items.Count;
        }

        public JsonItemSet Clone() {
            JsonItemSet itemSet = new JsonItemSet();
            itemSet.SupportCount = SupportCount;
            foreach (JsonItem anItem in items) {
                itemSet.AddItem(anItem.Clone());
            }
            return itemSet;
        }

        public string GetInfoString() {
            string info = "";

            foreach (JsonItem anItem in items) {
                info += (" " + anItem.Symbol);
            }

            return info;
        }

        public void Print() {
            Console.WriteLine(SupportCount);
            foreach (JsonItem item in items) {
                Console.Write(item.Symbol + " ");
            }
            Console.WriteLine();
        }

        public JsonItem GetLastItem() {
            return items.Last();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            if (items.Count > 0) {
                foreach (var item in items) {
                    sb.Append(item).Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
            }
            return string.Format("[SupportCount={0} {1}]", SupportCount, sb);
        }
    }
}
