using Octokit;

namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Newtonsoft.Json;

    [Table("minyar.pull_requests")]
    public partial class pull_requests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pull_requests(PullRequest pull, int repoId)
        {
            original_id = pull.Number;
            repository_id = repoId;
            number = pull.Number;
            state = pull.State.ToString();
            title = pull.Title;
            body = pull.Body;
            base_sha = pull.Base.Sha;
            merged_commit_sha = pull.MergeCommitSha;
            original_created_at = pull.CreatedAt.ToString("s");
            original_updated_at = pull.UpdatedAt.ToString("s");
            raw_json = JsonConvert.SerializeObject(pull);
            review_comments1 = new HashSet<review_comments>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pull_requests() {
            review_comments1 = new HashSet<review_comments>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int original_id { get; set; }

        public int repository_id { get; set; }

        public int number { get; set; }

        [Required]
        [StringLength(45)]
        public string state { get; set; }

        [StringLength(255)]
        public string title { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string body { get; set; }

        [Required]
        [StringLength(40)]
        public string base_sha { get; set; }

        [StringLength(40)]
        public string merged_commit_sha { get; set; }

        public int comments { get; set; }

        public int review_comments { get; set; }

        public int commits { get; set; }

        public int additions { get; set; }

        public int deletions { get; set; }

        public int changed_files { get; set; }

        [StringLength(45)]
        public string original_created_at { get; set; }

        [StringLength(45)]
        public string original_updated_at { get; set; }

        [StringLength(45)]
        public string closed_at { get; set; }

        [StringLength(45)]
        public string merged_at { get; set; }

        [StringLength(16777215)]
        public string raw_json { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime updated_at { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created_at { get; set; }

        [ForeignKey("repository_id")]
        public virtual repository repository { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<review_comments> review_comments1 { get; set; }
    }
}
