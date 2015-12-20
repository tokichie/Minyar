namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("minyar.diffs")]
    public partial class diff
    {
        public int id { get; set; }

        [Column("diff")]
        [Required]
        [StringLength(16777215)]
        public string diff1 { get; set; }

        public int? base_commit_id { get; set; }

        [Required]
        [StringLength(40)]
        public string base_sha { get; set; }

        public int? head_commit_id { get; set; }

        [Required]
        [StringLength(40)]
        public string head_sha { get; set; }

        [Required]
        [StringLength(260)]
        public string path { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime updated_at { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created_at { get; set; }

        public virtual commit commit { get; set; }

        public virtual commit commit1 { get; set; }
    }
}
