using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Minyar.Database;
using Newtonsoft.Json;

namespace Minyar.MachineLearning {
    public static class MlCache {
        public static List<repository> RepoCache;
        public static List<pull_requests> PullCache;
        public static Dictionary<string, List<double>> FeatureCache;

        static MlCache() {
            FeatureCache = new Dictionary<string, List<double>>();    
        }

        public static void Fetch() {
            using (var model = new MinyarModel()) {
                RepoCache = model.repositories.ToList();
                PullCache = model.pull_requests.ToList();
            }
        }

        public static bool Exists(string url) {
            return FeatureCache.ContainsKey(url);
        }

        public static void Add(string url, List<double> item) {
            FeatureCache[url] = item;
        }

        public static List<double> Get(string url) {
            return FeatureCache[url];
        } 
    }
}
