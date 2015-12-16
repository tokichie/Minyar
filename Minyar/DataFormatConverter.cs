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
            var res = new List<ItemTidSet<string, RepeatableTid>>();
            int tid = 0;
            foreach (var item in data) {
                tid++;
                var counter = new Dictionary<int, Dictionary<string, int>>(); 
                foreach (var symbol in item.Items.Select(i => i.Symbol)) {
                    counter[tid] = new Dictionary<string, int>();
                    if (!counter[tid].ContainsKey(symbol)) {
                        counter[tid][symbol] = 0;
                    } else {
                        counter[tid][symbol]++;
                    }
                }
                res.Add(new ItemTidSet<string, RepeatableTid>(new [] {item.}));
            }
        }
    }
}
