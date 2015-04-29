using System;
using Octokit;
using System.IO;
using System.Threading.Tasks;

namespace Minyar.Github {
	public static class OctokitClient {
        private static GitHubClient client;

        public static GitHubClient Client {
            get {
                if (client == null) {
                    SetClient();
                }
                return client;
            }
        }

        private static void SetClient() {
            client = new GitHubClient(new ProductHeaderValue(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name));
		    client.Credentials = new Credentials(IncludeToken());
        }

		public static string IncludeToken() {
			var token = "";
			try {
				var path = Path.Combine("..", "..", "Resources", "AuthInfo.txt");
				var reader = new StreamReader(path);
				token = reader.ReadLine().Trim();
			} catch (IOException e) {
				Console.WriteLine(e.StackTrace);
			}
			return token;
		}
	}
}

