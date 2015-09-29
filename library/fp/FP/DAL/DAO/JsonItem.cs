using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace FP.DAO {
	public class JsonItem {
		public int SupportCount;
        [DataMember(Name = "Opration")]
        public string Operation;
        [DataMember(Name = "NodeType")]
        public string NodeType;
        [DataMember(Name = "OriginalToken")]
        public string OriginalToken;
        [DataMember(Name = "ChangedToken")]
        public string ChangedToken;
        [DataMember(Name = "OriginalPosition")]
        public string OriginalPosition;
        [DataMember(Name = "ChangedPosition")]
        public string ChangedPosition;

        [DataMember(Name = "OriginalPath")]
        public string OriginalPath;
        [DataMember(Name = "ChangedPath")]
        public string ChangedPath;
	    public string GithubUrl;

	    public string Symbol {
	        get {
                var sb = new StringBuilder();
	            sb.Append(Operation).Append(":").Append(NodeType);
	            return sb.ToString();
	        }
	    }

		public JsonItem() {
		    SupportCount = -1;
		}

	    public JsonItem(int supportCount) {
	        SupportCount = supportCount;
	    }

	    public static JsonItem Deserialize(string json) {
	        var serializer = new JavaScriptSerializer();
	        return serializer.Deserialize<JsonItem>(json);
	    }

	    public JsonItem Clone() {
	        JsonItem item = new JsonItem();
	        item.SupportCount = SupportCount;
	        item.Operation = Operation;
	        item.NodeType = NodeType;
	        item.OriginalToken = OriginalToken;
	        item.ChangedToken = ChangedToken;
	        item.OriginalPosition = OriginalPosition;
	        item.ChangedPosition = ChangedPosition;
	        return item;
	    }

	    public override string ToString() {
	        var sb = new StringBuilder();
	        sb.Append("<").Append(Operation).Append(":").Append(NodeType);
	        var tokenChange = new StringBuilder();
	        var positionChange = new StringBuilder();
	        if (Operation == "Insert") {
	            tokenChange.Append("|").Append(ChangedToken);
                positionChange.Append(" ").Append(ChangedPosition);
	        } else if (Operation == "Update") {
	            tokenChange.Append("|").Append(OriginalToken).Append(" -> ").Append(ChangedToken);
                positionChange.Append("|").Append(OriginalPosition).Append(" -> ").Append(ChangedPosition);
            } else if (Operation == "Move") {
                tokenChange.Append("|").Append(OriginalToken);
                positionChange.Append("|").Append(OriginalPosition).Append(" -> ").Append(ChangedPosition);
            } else if (Operation == "Delete") {
                tokenChange.Append("|").Append(OriginalToken);
                positionChange.Append("|").Append(ChangedPosition);
            }
            sb.Append("|").Append(OriginalPath).Append(tokenChange.ToString().Trim())
                .Append(positionChange.ToString().Trim()).Append(">");
	        return sb.ToString();
	    }
	}
}

