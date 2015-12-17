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
        public int Threshold;

        public List<ItemTidSet<string, RepeatableTid>> ClosedItemSets; 

        public ItTreeMiner(string inputPath) {
            inputFilePath = inputPath;
            Threshold = -1;
        }

        public ItTreeMiner(string inputPath, int threshold) {
            inputFilePath = inputPath;
            Threshold = threshold;
        }

        public void GenerateClosedItemSets() {
            int lineCount = 0;
            var data = new List<ItemWrapper>();
            using (var reader = new StreamReader(inputFilePath)) {
                foreach (var line in reader.ReadLines()) {
                    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
                    data.Add(itemWrapper);
                    lineCount++;
                }
            }
            if (Threshold == -1) Threshold = lineCount / 10 * 4;
            var convertedData = DataFormatConverter.HorizontalToVertical(data);
            var itTree = new ItTree(convertedData, Threshold);
            ClosedItemSets = itTree.GetClosedItemSets().OrderByDescending(i => i.SupportCount);
        }

        public List<ItemTidSet<string, RepeatableTid>> GetMinedItemSets() {
            return ClosedItemSets;
        } 
    }
}
