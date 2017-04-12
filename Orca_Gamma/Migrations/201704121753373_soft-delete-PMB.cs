namespace Orca_Gamma.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class softdeletePMB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrivateMessageBetweens", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrivateMessageBetweens", "IsDeleted");
        }
    }
}
