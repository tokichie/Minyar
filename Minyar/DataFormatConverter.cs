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
                var repeatCounter = new Dictionary<string, int>();
                foreach (var symbol in item.Items.Select(i => i.Symbol)) {
                    if (!repeatCounter.ContainsKey(symbol)) {
                        repeatCounter[symbol] = 1;
                    } else {
                        repeatCounter[symbol]++;
                    }
                    var _symbol = string.Format("{0}:{1}", symbol, repeatCounter[symbol]);
                    if (!counter.ContainsKey(_symbol)) {
                        counter[_symbol] = new Dictionary<int, int>();
                    }
                    if (!counter[_symbol].ContainsKey(tid)) {
                        counter[_symbol][tid] = 1;
                    } else {
                        counter[_symbol][tid]++;
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
