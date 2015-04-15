using System;
using Code2Xml.Core.SyntaxTree;
using System.Collections.Generic;

namespace Minyar {
    public struct ChangePair {
        public CstChangeOperation Operation;
        public string NodeType;
        public string OriginalToken;
        public string ChangedToken;

        public ChangePair(CstChangeOperation op, CstNode orgNode = null, CstNode cmpNode = null) {
            this.Operation = op;
            this.NodeType = "";
            this.OriginalToken = "";
            this.ChangedToken = "";
            if (orgNode != null) {
                this.NodeType = orgNode.Name;
                this.OriginalToken = orgNode.TokenText;
            }
            if (cmpNode != null) {
                this.ChangedToken = cmpNode.TokenText;
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
    }
}

