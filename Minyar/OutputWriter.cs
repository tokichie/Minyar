using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ikvm.extensions;

namespace Minyar {
    class OutputWriter {
        private StreamWriter writer;

        public OutputWriter(string path) {
            try {
                writer = new StreamWriter(path);
            } catch (IOException e) {
                Console.WriteLine(e);
            }
        }

        public void AppendLine(string line) {
            writer.WriteLine(line);
        }

        public void Close() {
            writer.Close();
        }

        public void Dispose() {
            writer.Dispose();
        }
    }
}
