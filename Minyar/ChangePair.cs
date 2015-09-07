using System;
using Code2Xml.Core.SyntaxTree;
using System.Collections.Generic;

namespace Minyar {
	public struct ChangePair {
		public CstChangeOperation Operation;
		public string NodeType;
		public string OriginalToken;
		public string ChangedToken;

		public ChangePair(CstChangeOperation op, AstNode orgNode = null, AstNode cmpNode = null) {
			this.Operation = op;
			this.NodeType = "";
			this.OriginalToken = "";
			this.ChangedToken = "";
			if (orgNode != null) {
				this.NodeType = orgNode.Name;
				this.OriginalToken = orgNode.Token.Text;
			}
			if (cmpNode != null) {
				this.ChangedToken = cmpNode.Token.Text;
				if (op == CstChangeOperation.Insert)
					this.NodeType = cmpNode.Name;
			}
		}

		public ChangePair(CstChangeOperation op, string nodeType, string orgToken, string cmpToken) {
			this.Operation = op;
			this.NodeType = nodeType;
			this.OriginalToken = orgToken;
			this.ChangedToken = cmpToken;
		}

		public override string ToString() {
			var res = "";
			if (NodeType == "TOKEN")
				res = string.Format("<{0}:TOKEN:{1}:{2}>", Operation.ToString(), OriginalToken, ChangedToken);
			else
				res = string.Format("<{0}:{1}>", Operation.ToString(), NodeType);
			return res;
		}
	}
}

