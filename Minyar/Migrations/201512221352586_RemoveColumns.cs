namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveColumns : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropForeignKey("files", "commit_sha", "commits");
            DropForeignKey("diffs", "head_sha", "commits");
            DropForeignKey("diffs", "base_sha", "commits");
            DropIndex("files", new[] { "commit_sha" });
            DropIndex("diffs", new[] { "head_sha" });
            DropIndex("diffs", new[] { "base_sha" });
            DropPrimaryKey("commits");
            AlterColumn("files", "commit_sha", c => c.String(maxLength: 40, unicode: false));
            AlterColumn("files", "commit_sha", c => c.Int());
            AlterColumn("diffs", "head_sha", c => c.Int());
            AlterColumn("diffs", "base_sha", c => c.Int());
            AddPrimaryKey("commits", "id");
            RenameColumn(table: "files", name: "commit_sha", newName: "commit_id");
            RenameColumn(table: "diffs", name: "head_sha", newName: "head_commit_id");
            RenameColumn(table: "diffs", name: "base_sha", newName: "base_commit_id");
            AddColumn("files", "commit_sha", c => c.String(maxLength: 40, unicode: false));
            AddColumn("diffs", "head_sha", c => c.String(nullable: false, maxLength: 40, unicode: false));
            AddColumn("diffs", "base_sha", c => c.String(nullable: false, maxLength: 40, unicode: false));
            CreateIndex("files", "commit_id");
            CreateIndex("diffs", "head_commit_id");
            CreateIndex("diffs", "base_commit_id");
        }
    }
}
