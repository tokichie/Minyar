using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Minyar {
    class ApiRateLimit {
        private static readonly string token = "a23e7ecd104c8ae7e66f68cb53840facaadc8a5d";
        private static readonly string rateLimitUrl = 
            string.Format("https://api.github.com/rate_limit?access_token={0}", token);

        public static LimitObject RateLimit;

        public class LimitObject {
            public int Limit;
            public int Remaining;
            public int Reset;
            public TimeSpan UntilResetTime;

            public LimitObject(int limit, int remaining, int reset) {
                Limit = limit;
                Remaining = remaining;
                Reset = reset;
                var now = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                UntilResetTime = new TimeSpan(0, 1, reset - (int) now.TotalSeconds);
            }
        }

        public static void CheckLimit() {
            var request = WebRequest.Create(rateLimitUrl) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = WebRequestMethods.Http.Get;
            //request.Headers.Add("Authorization", string.Format("Token {0}", token));
            //request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Minyar";
            var response = (HttpWebResponse) request.GetResponse();
            string json = "";
            using (var sr = new StreamReader(response.GetResponseStream())) {
                json = sr.ReadToEnd();
            }
            var parsedJson = JObject.Parse(json);
            var limitJson = parsedJson["resources"]["core"];
            RateLimit = JsonConvert.DeserializeObject<LimitObject>(limitJson.ToString());
            CheckApiRemaining();
        }

        private static void CheckApiRemaining() {
            Console.WriteLine("[Info] API remaining count is {0}", RateLimit.Remaining);
            if (RateLimit.Remaining < 100) {
                Console.WriteLine(
                    "[Info] Waiting API reset until {0}", DateTime.Now.Add(RateLimit.UntilResetTime));
                System.Threading.Thread.Sleep(RateLimit.UntilResetTime);
            }
        }
    }
}
