using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    static class FileCache {
        private static readonly string cacheDir = Path.Combine("..", "..", "..", "cache");

        public static bool FileExists(string owner, string name, string sha, string path) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            return File.Exists(filepath);
        }

        public static string LoadFile(string owner, string name, string sha, string path) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            if (File.Exists(filepath)) {
                return new StreamReader(filepath).ReadToEnd();
            }
            throw new FileNotFoundException();
        }

        public static void SaveFile(string owner, string name, string sha, string path, string content) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            Directory.CreateDirectory(Path.Combine(filepath, ".."));
            using (var writer = new StreamWriter(filepath)) {
                writer.Write(content);
            }
        }
    }
}
