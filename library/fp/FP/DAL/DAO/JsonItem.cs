using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace FP.DAO {
	public class JsonItem {
		public int SupportCount;
        [DataMember(Name = "Symbol")]
	    public string Symbol;
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

		public JsonItem() {
		    SupportCount = -1;
		}

	    public JsonItem(string symbol, int supportCount) {
	        Symbol = symbol;
	        SupportCount = supportCount;
	    }

	    public JsonItem Deserialize(string json) {
	        var serializer = new JavaScriptSerializer();
	        return serializer.Deserialize<JsonItem>(json);
	    }

	    public JsonItem Clone() {
	        JsonItem item = new JsonItem();
	        item.SupportCount = SupportCount;
	        item.Symbol = Symbol;
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
	            tokenChange.Append(" ").Append(ChangedToken);
                positionChange.Append(" ").Append(ChangedPosition);
	        } else if (Operation == "Update") {
	            tokenChange.Append(" ").Append(OriginalToken).Append(" -> ").Append(ChangedToken);
                positionChange.Append(" ").Append(OriginalPosition).Append(" -> ").Append(ChangedPosition);
            } else if (Operation == "Move") {
                tokenChange.Append(" ").Append(OriginalToken);
                positionChange.Append(" ").Append(OriginalPosition).Append(" -> ").Append(ChangedPosition);
            } else if (Operation == "Delete") {
                tokenChange.Append(" ").Append(OriginalToken);
                positionChange.Append(" ").Append(ChangedPosition);
            }
            sb.Append(tokenChange.ToString().Trim())
                .Append(positionChange.ToString().Trim()).Append(">");
	        return sb.ToString();
	    }
	}
}

