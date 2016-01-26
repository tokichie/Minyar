namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnToCommits : DbMigration
    {
        public override void Up()
        {
            AddColumn("commits", "parent_sha", c => c.String(maxLength: 40, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("commits", "parent_sha");
        }
    }
}
