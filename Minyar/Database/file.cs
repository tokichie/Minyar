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
        public file(string sha, string path, string content)
        {
            commit_sha = sha;
            this.path = path;
            this.content = content;
        }

        public file()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [StringLength(40)]
        public string commit_sha { get; set; }

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

        [ForeignKey("commit_sha")]
        public virtual commit commit { get; set; }
    }
}
