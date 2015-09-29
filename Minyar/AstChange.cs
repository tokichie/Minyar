using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Minyar {
    public class AstChange {
        [DataMember(Name = "GithubUrl")] public string GithubUrl;
        [DataMember(Name = "ChangeSet")] public HashSet<ChangePair> ChangeSet;

        public AstChange(string githubUrl, HashSet<ChangePair> changeSet) {
            GithubUrl = githubUrl;
            ChangeSet = changeSet;
        }

        public override string ToString() {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }
    }
}
