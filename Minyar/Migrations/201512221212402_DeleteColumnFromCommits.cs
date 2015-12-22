namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteColumnFromCommits : DbMigration
    {
        public override void Up()
        {
            DropColumn("commits", "original_id");
        }
        
        public override void Down()
        {
            AddColumn("commits", "original_id", c => c.Int(nullable: false));
        }
    }
}
