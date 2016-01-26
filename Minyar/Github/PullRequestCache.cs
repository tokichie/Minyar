using System.Linq;
using System.Threading.Tasks;
using Minyar.Database;

namespace Minyar.Github {
    static class PullRequestCache {
        public static bool PullExists(string owner, string name, int number) {
            using (var model = new MinyarModel()) {
                return model.pull_requests.Any(p => p.repository.full_name == owner + "/" + name && p.number == number);
            }
        }

        public static async Task<pull_requests> LoadPullFromDatabase(string owner, string name, int number) {
            using (var model = new MinyarModel()) {
                if (PullExists(owner, name, number)) {
                    return
                        model.pull_requests.First(
                            p => p.repository.full_name == owner + "/" + name && p.number == number);
                }
                var pull = await OctokitClient.Client.Repository.PullRequest.Get(owner, name, number);
                ApiRateLimit.CheckLimit();
                var repo = model.repositories.First(r => r.full_name == owner + "/" + name);
                var pr = new pull_requests(pull, repo.original_id);
                model.pull_requests.Add(pr);
                model.SaveChanges();
                return pr;
            }
        }
    }
}
