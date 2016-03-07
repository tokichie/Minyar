using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;
using Minyar.Charm;
using Minyar.Extensions;
using Paraiba.IO;

namespace Minyar {
    public class ItTreeMiner {
        private ItTree itTree;
        private string inputFilePath;
        private int absoluteThreshold;
        private int dynamicThreshold;
        private int freqMin;
        private int freqMax;

        public List<ItemTidSet<string, RepeatableTid>> ClosedItemSets; 

        public ItTreeMiner(string inputPath) {
            inputFilePath = inputPath;
        }

        public ItTreeMiner(string inputPath, int absoluteThreshold, int dynamicThreshold, int freqMin, int freqMax = 0) {
            inputFilePath = inputPath;
            this.absoluteThreshold = absoluteThreshold;
            this.dynamicThreshold = dynamicThreshold;
            this.freqMin = freqMin;
            this.freqMax = freqMax;
        }

        public void GenerateClosedItemSets() {
            int lineCount = 0;
            var data = new List<ItemWrapper>();
            using (var reader = new StreamReader(inputFilePath)) {
                foreach (var line in reader.ReadLines()) {
                    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
                    //if (itemWrapper.Items.Count < 4) continue;
                    data.Add(itemWrapper);
                    lineCount++;
                }
            }
            var convertedData = DataFormatConverter.HorizontalToVertical(data);
            var itTree = new ItTree(convertedData, absoluteThreshold, dynamicThreshold, freqMin, freqMax);
            ClosedItemSets = itTree.GetClosedItemSets().OrderByDescending(i => i.SupportCount).ToList();
        }

        public List<ItemTidSet<string, RepeatableTid>> GetMinedItemSets() {
            return ClosedItemSets;
        } 
    }
}
