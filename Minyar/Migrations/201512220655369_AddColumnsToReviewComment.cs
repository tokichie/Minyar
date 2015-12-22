namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnsToReviewComment : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropColumn("review_comments", "is_closed_pr");
            DropColumn("review_comments", "for_diff");
        }
    }
}
