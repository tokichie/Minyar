namespace Minyar.Database {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MinyarModel : DbContext {
        public MinyarModel()
            : base("name=MinyarEntity") {
        }

        public virtual DbSet<mined_itemsets> mined_itemsets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
        }
    }
}
