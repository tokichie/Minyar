using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FP.DAO;
using Paraiba.Core;

namespace FP.DAL.DAO {
    public class ItemWrapper {
        [DataMember(Name = "Items")] public List<JsonItem> Items;
        [DataMember(Name = "OriginalPath")] public string OriginalPath;
        [DataMember(Name = "ChangedPath")] public string ChangedPath;
        [DataMember(Name = "GithubUrl")] public string GithubUrl;
        [DataMember(Name = "DiffHunk")] public string DiffHunk;

        public static ItemWrapper Deserialize(string json) {
	        var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 10000000;
	        return serializer.Deserialize<ItemWrapper>(json);
        }

        public override bool Equals(object obj) {
            var right = obj as ItemWrapper;
            if (right == null) return false;
            if (Items.Count == right.Items.Count && GithubUrl == right.GithubUrl &&
                DiffHunk.SubstringAfter("@@ ") == right.DiffHunk.SubstringAfter("@@ ")) return true;
            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            var diffhunk = DiffHunk.SubstringAfter("@@ ").SubstringBefore("\n");
            var sb = new StringBuilder();
            sb.Append(GithubUrl).Append("@@ ").Append(diffhunk);
            return sb.ToString();
        }
    }
}
