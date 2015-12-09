using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar {
    static class Logger {
        private static string logDir;
        private static StreamWriter logger;
        private static bool active;

        static Logger() {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            logDir = Path.Combine("..", "..", "..", "logs", timestamp);
            Directory.CreateDirectory(logDir);
            logger = new StreamWriter(Path.Combine(logDir, "log.txt"));
            logger.AutoFlush = true;
            active = true;
        }

        public static void Activate() {
            active = true;
        }

        public static void Deactivate() {
            active = false;
        }

        public static void Error(string msg,
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0) {
            if (!active) return;
            var timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            logger.WriteLine("{0} [ERROR] {1}:{2} {3}", timestamp, filePath, lineNumber, msg);
        }

        public static void Info(string msg, params object[] prms) {
            if (!active) return;
            var timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var formattedMsg = string.Format(msg, prms);
            logger.WriteLine("{0} [INFO] {1}", timestamp, formattedMsg);
        }
    }
}
