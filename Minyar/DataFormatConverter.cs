using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;
using Minyar.Charm;

namespace Minyar {
    public class DataFormatConverter {
        public static List<ItemTidSet<string, RepeatableTid>> HorizontalToVertical(List<ItemWrapper> data) {
            var counter = new Dictionary<string, Dictionary<int, int>>(); 
            int tid = 0;
            foreach (var item in data) {
                tid++;
                foreach (var symbol in item.Items.Select(i => i.Symbol)) {
                    if (!counter.ContainsKey(symbol)) {
                        counter[symbol] = new Dictionary<int, int>();
                    } else {
                        if (!counter[symbol].ContainsKey(tid)) {
                            counter[symbol][tid] = 1;
                        } else {
                            counter[symbol][tid]++;
                        }
                    }
                }
            }
            var res = new List<ItemTidSet<string, RepeatableTid>>();
            foreach (var pair in counter) {
                var symbol = pair.Key;
                var items = pair.Value;
                res.Add(new ItemTidSet<string, RepeatableTid>(
                    new[] {symbol},
                    items.Select(i => new RepeatableTid(i.Key, i.Value))
                    ));
            }
            return res;
        }
    }
}
