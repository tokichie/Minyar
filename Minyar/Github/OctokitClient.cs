using System;
using Octokit;
using System.IO;
using System.Threading.Tasks;

namespace Minyar.Github {
	public static class OctokitClient {
        private static GitHubClient client;
	    private static string token;

        public static GitHubClient Client {
            get {
                if (client == null) {
                    SetClient();
                }
                return client;
            }
        }

	    public static string Token {
	        get {
	            if (token == null) {
	                token = IncludeToken();
	            }
	            return token;
;	        }
	    }


        private static void SetClient() {
            client = new GitHubClient(new ProductHeaderValue(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name));
            client.Credentials = new Credentials(Token);
        }

		private static string IncludeToken() {
			var token = "";
			try {
				var path = Path.Combine("..", "..", "..", "Minyar", "Resources", "AuthInfo.txt");
				var reader = new StreamReader(path);
				token = reader.ReadLine().Trim();
			} catch (IOException e) {
				Console.WriteLine(e.StackTrace);
			}
			return token;
		}
	}
}

