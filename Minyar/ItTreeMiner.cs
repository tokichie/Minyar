using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;
using Minyar.Charm;
using Paraiba.IO;

namespace Minyar {
    public class ItTreeMiner {
        private ItTree itTree;
        private string inputFilePath;
        public int Threshold;

        public ItTreeMiner(string inputPath, int threshold = -1) {
            Threshold = threshold;
            inputFilePath = inputPath;
        }

        public void GenerateClosedItemsets() {
            int lineCount = 0;
            using (var reader = new StreamReader(inputFilePath)) {
                lineCount++;
                var data = new List<ItemWrapper>();
                foreach (var line in reader.ReadLines()) {
                    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
                    data.Add(itemWrapper);
                }
            }
            if (Threshold == -1) Threshold = lineCount / 10 * 2;
        }
    }
}
