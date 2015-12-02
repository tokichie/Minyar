using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar {
    static class Logger {
        private static string logDir;
        private static StreamWriter err, info;

        static Logger() {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            logDir = Path.Combine("..", "..", "..", "logs", timestamp);
            Directory.CreateDirectory(logDir);
            err = new StreamWriter(Path.Combine(logDir, "error.log"));
            info = new StreamWriter(Path.Combine(logDir, "info.log"));
            err.AutoFlush = true;
            info.AutoFlush = true;
        }

        public static void Error(string msg,
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0) {
            var timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            err.WriteLine("{0} {1}:{2} {3}", timestamp, filePath, lineNumber, msg);
        }

        public static void Info(string msg, params object[] prms) {
            var timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var formattedMsg = string.Format(msg, prms);
            info.WriteLine("{0} {1}", timestamp, formattedMsg);
        }
    }
}
