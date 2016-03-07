using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;
using FP.DAO;

namespace Minyar {
    class PatternMatcher {
        private HashSet<string> patternSet;
        private string filePath;

        public List<FP.DAL.DAO.ItemWrapper> MatchedItems; 

        public PatternMatcher(string path, HashSet<string> pattern) {
            filePath = path;
            patternSet = pattern;
            MatchedItems = new List<FP.DAL.DAO.ItemWrapper>();
        }

        public void Match() {
            if (MatchedItems.Count > 0) return;
            using (var file = new StreamReader(filePath)) {
                var line = "";
                while ((line = file.ReadLine()) != null) {
                    var itemWrapper = FP.DAL.DAO.ItemWrapper.Deserialize(line.Trim());
                    var set = new HashSet<string>();
                    foreach (var item in itemWrapper.Items) {
                        if (!set.Contains(item.Symbol))
                            set.Add(item.Symbol);
                    }
                    if (patternSet.IsSubsetOf(set)) {
                        MatchedItems.Add(itemWrapper);
                    }
                }
            }
        }
    }
}
