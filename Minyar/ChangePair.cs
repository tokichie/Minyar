using System;
using Code2Xml.Core.SyntaxTree;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Minyar {
	public class ChangePair {
	    [DataMember(Name = "Opration")] public CstChangeOperation Operation;

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

		public ChangePair(CstChangeOperation op, string filePath, AstNode orgNode = null, AstNode cmpNode = null) {
			this.Operation = op;
			this.NodeType = "";
			this.OriginalToken = "";
			this.ChangedToken = "";
			this.OriginalPath = "";
			this.ChangedPath = filePath;
			this.OriginalPosition = "";
			this.ChangedPosition = "";
			if (orgNode != null) {
				if (orgNode.HasToken) {
					this.OriginalToken = orgNode.Token.Text;
					this.OriginalPosition = orgNode.Token.Range.ToString().Replace(" ", "");
				}
				if (op != CstChangeOperation.Insert)
					this.NodeType = orgNode.Name;
			}
			if (cmpNode != null) {
				if (cmpNode.HasToken) {
					this.ChangedToken = cmpNode.Token.Text;
					this.ChangedPosition = cmpNode.Token.Range.ToString().Replace(" ", "");
				}
				if (op == CstChangeOperation.Insert)
					this.NodeType = cmpNode.Name;
			}
		}

        public ChangePair() { }

		public ChangePair(CstChangeOperation op, string filePath, string orgToken, string cmpToken) {
			this.Operation = op;
			this.NodeType = "";
			this.OriginalPath = "";
			this.OriginalPosition = "";
			this.ChangedPosition = "";
			this.ChangedPath = filePath;
			this.OriginalToken = orgToken;
			this.ChangedToken = cmpToken;
		}

		public override string ToString() {
            //var res = string.Format(
            //        "<{0}:{1}|{2}:{3}:{4}:{5}:{6}>",
            //        Operation,
            //        NodeType,
            //        OriginalToken,
            //        ChangedToken,
            //        ChangedPath,
            //        OriginalPosition,
            //        ChangedPosition);
            var res = string.Format(
                "<{0}:{1}>",
                Operation,
                NodeType);
            return res;
		}
	}
}

