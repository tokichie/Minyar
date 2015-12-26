using System.Linq;
using Newtonsoft.Json;
using Octokit;

namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("minyar.commits")]
    public partial class commit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public commit(GitHubCommit commit, int repoId) {
            repository_id = repoId;
            sha = commit.Sha;
            parent_sha = commit.Parents[0].Sha;
            url = commit.Url;
            html_url = commit.HtmlUrl;
            comments_url = commit.CommentsUrl;
            raw_json = JsonConvert.SerializeObject(commit);
            diffs = new HashSet<diff>();
            files = new HashSet<file>();
            diffs1 = new HashSet<diff>();
        }

        public IEnumerable<GitHubCommitFile> GetFiles() {
            var commit = JsonConverter.Deserialize<GitHubCommit>(raw_json);
            return commit.Files;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public commit()
        {
            diffs = new HashSet<diff>();
            files = new HashSet<file>();
            diffs1 = new HashSet<diff>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int repository_id { get; set; }

        [Key]
        [Required]
        [StringLength(40)]
        public string sha { get; set; }

        [StringLength(40)]
        public string parent_sha { get; set; }
        
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

        [ForeignKey("repository_id")]
        public virtual repository repository { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<file> files { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<diff> diffs1 { get; set; }
    }
}
