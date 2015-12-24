using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Minyar.Database {
    using System.Data.Entity;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class MinyarModel : DbContext {
        //public static readonly string credentialPath = Path.Combine("..", "..", "..", "Minyar", "Resources",
        //    "credentials.json");
        public static readonly string credentialPath = @"C:\repos\Minyar\Minyar\Resources\credentials.json";

        public MinyarModel() : base(FormatConnectionString()) {
        }

        private static string FormatConnectionString() {
            var connString = ConfigurationManager.ConnectionStrings["MinyarModelConnection"].ConnectionString;
            var builder = new MySqlConnectionStringBuilder(connString);
            builder.ConvertZeroDateTime = true;
            using (var reader = new StreamReader(credentialPath)) {
                var json = JsonConvert.DeserializeObject(reader.ReadToEnd()) as JObject;
                builder.Server = json["Database"]["Server"].ToString();
                builder.UserID = json["Database"]["UserId"].ToString();
                builder.Password = json["Database"]["Password"].ToString();
                builder.Database = json["Database"]["DatabaseName"].ToString();
            }
            return builder.ToString();
        }

        public virtual DbSet<commit> commits { get; set; }
        public virtual DbSet<diff> diffs { get; set; }
        public virtual DbSet<file> files { get; set; }
        public virtual DbSet<pull_requests> pull_requests { get; set; }
        public virtual DbSet<repository> repositories { get; set; }
        public virtual DbSet<review_comments> review_comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<commit>()
                .Property(e => e.sha)
                .IsUnicode(false);

            modelBuilder.Entity<commit>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<commit>()
                .Property(e => e.html_url)
                .IsUnicode(false);

            modelBuilder.Entity<commit>()
                .Property(e => e.comments_url)
                .IsUnicode(false);

            modelBuilder.Entity<commit>()
                .Property(e => e.raw_json)
                .IsUnicode(false);

            modelBuilder.Entity<commit>()
                .HasMany(e => e.diffs)
                .WithRequired(e => e.commit)
                .HasForeignKey(e => e.base_sha);

            modelBuilder.Entity<commit>()
                .HasMany(e => e.files)
                .WithRequired(e => e.commit)
                .HasForeignKey(e => e.commit_sha);

            modelBuilder.Entity<commit>()
                .HasMany(e => e.diffs1)
                .WithRequired(e => e.commit1)
                .HasForeignKey(e => e.head_sha);

            modelBuilder.Entity<diff>()
                .Property(e => e.diff1)
                .IsUnicode(false);

            modelBuilder.Entity<diff>()
                .Property(e => e.base_sha)
                .IsUnicode(false);

            modelBuilder.Entity<diff>()
                .Property(e => e.head_sha)
                .IsUnicode(false);

            modelBuilder.Entity<diff>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<file>()
                .Property(e => e.commit_sha)
                .IsUnicode(false);

            modelBuilder.Entity<file>()
                .Property(e => e.content)
                .IsUnicode(false);

            modelBuilder.Entity<file>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<file>()
                .Property(e => e.download_url)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.title)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.body)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.base_sha)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.merged_commit_sha)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.original_created_at)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.original_updated_at)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.closed_at)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.merged_at)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .Property(e => e.raw_json)
                .IsUnicode(false);

            modelBuilder.Entity<pull_requests>()
                .HasMany(e => e.review_comments1)
                .WithRequired(e => e.pull_requests)
                .HasForeignKey(e => e.pull_request_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.full_name)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.default_branch)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.html_url)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.original_updated_at)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.original_created_at)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.pushed_at)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .Property(e => e.raw_json)
                .IsUnicode(false);

            modelBuilder.Entity<repository>()
                .HasMany(e => e.commits)
                .WithRequired(e => e.repository)
                .HasForeignKey(e => e.repository_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<repository>()
                .HasMany(e => e.pull_requests)
                .WithRequired(e => e.repository)
                .HasForeignKey(e => e.repository_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.body)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.diff_hunk)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.commit_id)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.original_commit_id)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.html_url)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.pull_request_url)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.original_updated_at)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.original_created_at)
                .IsUnicode(false);

            modelBuilder.Entity<review_comments>()
                .Property(e => e.raw_json)
                .IsUnicode(false);
        }
    }
}
