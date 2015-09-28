using System;
using Code2Xml.Core.SyntaxTree;
using System.Collections.Generic;

namespace Minyar {
	public struct ChangePair {
		public CstChangeOperation Operation;
		public string NodeType;
		public string OriginalToken;
		public string ChangedToken;
		public string OriginalPath;
		public string ChangedPath;
		public string OriginalPosition;
		public string ChangedPosition;

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
            var res = string.Format(
                    "<{0}:{1}|{2}:{3}:{4}:{5}:{6}>",
                    Operation,
                    NodeType,
                    OriginalToken,
                    ChangedToken,
                    ChangedPath,
                    OriginalPosition,
                    ChangedPosition);
            //var res = string.Format(
            //    "<{0}:{1}>",
            //    Operation,
            //    NodeType);
			return res;
		}
	}
}

