namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteColumnFromPullRequests : DbMigration
    {
        public override void Up()
        {
            DropColumn("pull_requests", "original_id");
        }
        
        public override void Down()
        {
            AddColumn("pull_requests", "original_id", c => c.Int(nullable: false));
        }
    }
}
