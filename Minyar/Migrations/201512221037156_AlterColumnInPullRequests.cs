namespace Minyar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumnInPullRequests : DbMigration
    {
        public override void Up()
        {
            AlterColumn("pull_requests", "title", c => c.String(maxLength: 1000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("pull_requests", "title", c => c.String(maxLength: 255, unicode: false));
        }
    }
}
