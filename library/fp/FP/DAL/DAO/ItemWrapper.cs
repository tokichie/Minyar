using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FP.DAO;

namespace FP.DAL.DAO {
    public class ItemWrapper {
        [DataMember(Name = "Items")] public List<JsonItem> Items;
        [DataMember(Name = "OriginalPath")] public string OriginalPath;
        [DataMember(Name = "ChangedPath")] public string ChangedPath;
        [DataMember(Name = "GithubUrl")] public string GithubUrl;
        [DataMember(Name = "DiffHunk")] public string DiffHunk;

        public static ItemWrapper Deserialize(string json) {
	        var serializer = new JavaScriptSerializer();
	        return serializer.Deserialize<ItemWrapper>(json);
        }
    }
}
