using Newtonsoft.Json;
using Octokit;

namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("minyar.repositories")]
    public partial class repository
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public repository(Repository repo) {
            raw_json = JsonConvert.SerializeObject(repo);
            original_id = repo.Id;
            name = repo.Name;
            full_name = repo.FullName;
            stargazers_count = repo.StargazersCount;
            forks_count = repo.ForksCount;
            default_branch = repo.DefaultBranch;
            url = repo.Url;
            html_url = repo.HtmlUrl;
            original_updated_at = repo.UpdatedAt.ToString("s");
            original_created_at = repo.CreatedAt.ToString("s");
            commits = new HashSet<commit>();
            pull_requests = new HashSet<pull_requests>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public repository() {
            commits = new HashSet<commit>();
            pull_requests = new HashSet<pull_requests>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int original_id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        [Required]
        [StringLength(255)]
        public string full_name { get; set; }

        public int? stargazers_count { get; set; }

        public int? watchers_count { get; set; }

        public int? subscribers_count { get; set; }

        public int? forks_count { get; set; }

        public int? size { get; set; }

        [StringLength(45)]
        public string default_branch { get; set; }

        [StringLength(2083)]
        public string url { get; set; }

        [StringLength(2083)]
        public string html_url { get; set; }

        [StringLength(45)]
        public string original_updated_at { get; set; }

        [StringLength(45)]
        public string original_created_at { get; set; }

        [StringLength(45)]
        public string pushed_at { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime created_at { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime updated_at { get; set; }

        [StringLength(16777215)]
        public string raw_json { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<commit> commits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pull_requests> pull_requests { get; set; }
    }
}
