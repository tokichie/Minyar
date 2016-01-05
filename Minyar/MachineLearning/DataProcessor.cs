using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord;
using FP.DAL.DAO;
using Minyar.Extensions;
using Paraiba.Collections.Generic;
using Paraiba.IO;

namespace Minyar.MachineLearning {
    public class DataProcessor {
        public class DataShortageException : Exception {
            
        }

        private string negativeFilePath;
        private string positiveFilePath;
        private List<ItemWrapper> negativeItems;
        private List<ItemWrapper> positiveItems;

        public List<HashSet<string>> NegativeItems {
            get { return negativeItems.Select(i => new HashSet<string>(i.Items.Select(j => j.Symbol))).ToList(); }
        } 

        public List<HashSet<string>> PositiveItems {
            get { return positiveItems.Select(i => new HashSet<string>(i.Items.Select(j => j.Symbol))).ToList(); }
        } 

        public DataProcessor(string negativePath, string positivePath) {
            negativeFilePath = negativePath;
            positiveFilePath = positivePath;
            negativeItems = new List<ItemWrapper>();
            positiveItems = new List<ItemWrapper>();
        }

        public void Sample(int count, int minItemCount) {
            SampleRandomly(negativeFilePath, ref negativeItems, count, minItemCount);
            SampleRandomly(positiveFilePath, ref positiveItems, count, minItemCount);
        }

        private void SampleRandomly(string path, ref List<ItemWrapper> itemList, int count, int minItemCount) {
            using (var reader = new StreamReader(path)) {
                var items = new List<ItemWrapper>();
                foreach (var line in reader.ReadLines()) { 
                    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
                    if (itemWrapper.Items.Count < minItemCount) continue;
                    items.Add(itemWrapper);
                }
                if (items.Count < count) throw new DataShortageException();
                itemList.AddRange(items.Shuffle().Take(count));
            }
        }
    }
}
