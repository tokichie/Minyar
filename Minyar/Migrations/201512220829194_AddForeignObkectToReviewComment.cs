namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignObkectToReviewComment : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropForeignKey("review_comments", "repository_id", "repositories");
        }
    }
}
