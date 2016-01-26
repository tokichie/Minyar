using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Octokit;

namespace Minyar.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("minyar.review_comments")]
    public partial class review_comments
    {
        public review_comments(PullRequestReviewComment comment, string repoOwner, string repoName) :
            this(comment, MinyarModel.GetRepositoryId(repoOwner, repoName)) { }

        public review_comments(PullRequestReviewComment comment, int? repoId = null) {
            original_id = comment.Id;
            repository_id = repoId;
            body = comment.Body;
            diff_hunk = comment.DiffHunk;
            path = comment.Path;
            position = comment.Position;
            original_position = comment.OriginalPosition;
            commit_id = comment.CommitId;
            original_commit_id = comment.OriginalCommitId;
            url = comment.Url.ToString();
            html_url = comment.HtmlUrl.ToString();
            pull_request_url = comment.PullRequestUrl.ToString();
            original_updated_at = comment.UpdatedAt.ToString("s");
            original_created_at = comment.CreatedAt.ToString("s");
            raw_json = JsonConvert.SerializeObject(comment);
        }

        public review_comments() {
            
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj) {
            var right = obj as review_comments;
            if (right == null) return false;
            return repository_id == right.repository_id &&
                   path == right.path && position == right.position &&
                   original_position == right.original_position &&
                   commit_id == right.commit_id &&
                   original_commit_id == right.original_commit_id;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(repository_id).Append(",")
                .Append(path).Append(",")
                .Append(position).Append(",")
                .Append(original_position).Append(",")
                .Append(commit_id).Append(",")
                .Append(original_commit_id);
            return sb.ToString();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int original_id { get; set; }

        public int? repository_id { get; set; }

        public int? pull_request_id { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(65535)]
        public string body { get; set; }

        [Required]
        [StringLength(16777215)]
        public string diff_hunk { get; set; }

        [Required]
        [StringLength(260)]
        public string path { get; set; }

        public int? position { get; set; }

        public int? original_position { get; set; }

        [StringLength(40)]
        public string commit_id { get; set; }

        [StringLength(40)]
        public string original_commit_id { get; set; }

        public int? for_diff { get; set; }

        public bool? is_closed_pr { get; set; }
        
        public bool? is_first_comment { get; set; }

        [StringLength(2083)]
        public string url { get; set; }

        [StringLength(2083)]
        public string html_url { get; set; }

        [StringLength(2083)]
        public string pull_request_url { get; set; }

        [StringLength(45)]
        public string original_updated_at { get; set; }

        [StringLength(45)]
        public string original_created_at { get; set; }

        [StringLength(16777215)]
        public string raw_json { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime updated_at { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created_at { get; set; }

        [ForeignKey("pull_request_id")]
        public virtual pull_requests pull_requests { get; set; }

        [ForeignKey("repository_id")]
        public virtual repository repository { get; set; }
    }
}
