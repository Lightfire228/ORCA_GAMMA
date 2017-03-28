namespace Orca_Gamma.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedthreadpmchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "DateStarted", c => c.DateTime(nullable: false));
            AddColumn("dbo.Projects", "DateFinished", c => c.DateTime(nullable: false));
            AddColumn("dbo.PrivateMessagePosts", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.PrivateMessagePosts", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PrivateMessagePosts", "IsImportant", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrivateMessagePosts", "IsImportant");
            DropColumn("dbo.PrivateMessagePosts", "IsDeleted");
            DropColumn("dbo.PrivateMessagePosts", "Date");
            DropColumn("dbo.Projects", "DateFinished");
            DropColumn("dbo.Projects", "DateStarted");
        }
    }
}
