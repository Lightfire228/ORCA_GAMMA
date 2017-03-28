namespace Orca_Gamma.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forgotdatejoined : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DateJoined", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Projects", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.PrivateMessagePosts", "Body", c => c.String(nullable: false));
            AlterColumn("dbo.PrivateMessages", "Subject", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PrivateMessages", "Subject", c => c.String());
            AlterColumn("dbo.PrivateMessagePosts", "Body", c => c.String());
            AlterColumn("dbo.Projects", "Description", c => c.String());
            AlterColumn("dbo.Projects", "Name", c => c.String());
            DropColumn("dbo.AspNetUsers", "DateJoined");
        }
    }
}
