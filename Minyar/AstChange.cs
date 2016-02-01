using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Paraiba.Core;

namespace Minyar {
    public class AstChange {
        [DataMember(Name = "GithubUrl")] public string GithubUrl { get; set; }
        [DataMember(Name = "Items")] public HashSet<ChangePair> Items { get; set; }
        [DataMember(Name = "DiffHunk")] public string DiffHunk { get; set; }
        public string Author { get; set; }
        public int Addition { get; set; }
        public int Deletion { get; set; }
        public bool OrgIsInnerOfMethod { get; set; }
        public bool CmpIsInnerOfMethod { get; set; }

        public AstChange(string githubUrl, HashSet<ChangePair> changeSet, string diffHunk) {
            GithubUrl = githubUrl;
            Items = changeSet;
            DiffHunk = diffHunk;
        }

        public AstChange() {
            
        }

        public override bool Equals(object obj) {
            var right = obj as AstChange;
            if (right == null) return false;
            if (Items.Count == right.Items.Count && GithubUrl == right.GithubUrl &&
                DiffHunk.SubstringAfter("@@ ") == right.DiffHunk.SubstringAfter("@@ ")) return true;
            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            var serializer = JsonSerializer.Create(settings);
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb)) {
                serializer.Serialize(sw, this);
            }
            return sb.ToString();
        }
    }
}
