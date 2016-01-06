namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnToReviewComents : DbMigration
    {
        public override void Up()
        {
            AddColumn("review_comments", "is_first_comment", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("review_comments", "is_first_comment");
        }
    }
}
