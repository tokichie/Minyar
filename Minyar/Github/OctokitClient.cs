using System;
using Octokit;
using System.IO;

namespace Minyar.Github {
	public class OctokitClient {
		private GitHubClient github;

		public OctokitClient() {
			github = new GitHubClient(new ProductHeaderValue("Minyar"));
			github.Credentials = new Credentials(IncludeToken());
		}

		private string IncludeToken() {
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

