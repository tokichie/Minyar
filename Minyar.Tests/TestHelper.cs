using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace Minyar.Tests {
    public static class TestHelper {
        public static void Download(string url, string outPath) {
            using (var client = new WebClient()) {
                using (var stream = client.OpenRead(url)) {
                    using (var file = new FileStream(outPath, FileMode.Create)) {
                        var buff = new byte[64 * 1024];
                        int len;
                        while ((len = stream.Read(buff, 0, buff.Length)) > 0) {
                            file.Write(buff, 0, len);
                        }
                    }
                }
            }
        }

        public static void Unzip(string inPath, string outDirPath) {
            var fastZip = new FastZip();
            fastZip.ExtractZip(inPath, outDirPath, null);
        }
    }
}