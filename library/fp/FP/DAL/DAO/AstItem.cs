namespace FP.DAL.DAO {
    public class AstItem {
        public string NodeName;
        public string Metadata;

        public AstItem(string nodeName, string metaData) {
            this.NodeName = nodeName;
            this.Metadata = metaData;
        }
    }
}