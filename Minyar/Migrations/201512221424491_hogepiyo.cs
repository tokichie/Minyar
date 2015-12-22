namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hogepiyo : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropPrimaryKey("minyar.files");
            DropPrimaryKey("minyar.diffs");
            AlterColumn("minyar.files", "id", c => c.Int(nullable: false));
            AlterColumn("minyar.diffs", "id", c => c.Int(nullable: false));
            AddPrimaryKey("minyar.files", "id");
            AddPrimaryKey("minyar.diffs", "id");
        }
    }
}
