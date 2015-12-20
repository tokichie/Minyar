namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("commits")]
    public partial class commit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public commit()
        {
            diffs = new HashSet<diff>();
            files = new HashSet<file>();
            diffs1 = new HashSet<diff>();
        }

        public int id { get; set; }

        public int original_id { get; set; }

        public int repository_id { get; set; }

        [Required]
        [StringLength(40)]
        public string sha { get; set; }

        [StringLength(2083)]
        public string url { get; set; }

        [StringLength(2083)]
        public string html_url { get; set; }

        [StringLength(2083)]
        public string comments_url { get; set; }

        [Required]
        [StringLength(16777215)]
        public string raw_json { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime updated_at { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<diff> diffs { get; set; }

        public virtual repository repository { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<file> files { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<diff> diffs1 { get; set; }
    }
}
