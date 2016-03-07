namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnToDiff : DbMigration
    {
        public override void Up()
        {
            AddColumn("diffs", "raw_json", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("diffs", "raw_json");
        }
    }
}
