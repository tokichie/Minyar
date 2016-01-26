namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hogehogehoge : DbMigration
    {
        public override void Up()
        {
            AlterColumn("commits", "id", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("commits", "id", c => c.Int(nullable: false));
        }
    }
}
