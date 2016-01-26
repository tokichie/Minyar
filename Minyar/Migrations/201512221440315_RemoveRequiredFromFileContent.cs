namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRequiredFromFileContent : DbMigration
    {
        public override void Up()
        {
            AlterColumn("files", "content", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("files", "content", c => c.String(nullable: false, unicode: false));
        }
    }
}
