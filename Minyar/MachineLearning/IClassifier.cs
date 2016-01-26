using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.Bayes;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

namespace Minyar.MachineLearning {
    public interface IClassifier {
        void Train();

        double Classify(DataProcessor.MlItem inputs);
    }
}
