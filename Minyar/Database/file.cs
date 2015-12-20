namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("minyar.files")]
    public partial class file
    {
        public int id { get; set; }

        public int? commit_id { get; set; }

        [StringLength(40)]
        public string commit_sha { get; set; }

        [Required]
        [StringLength(16777215)]
        public string content { get; set; }

        [Required]
        [StringLength(260)]
        public string path { get; set; }

        [StringLength(2083)]
        public string download_url { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime updated_at { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created_at { get; set; }

        public virtual commit commit { get; set; }
    }
}
