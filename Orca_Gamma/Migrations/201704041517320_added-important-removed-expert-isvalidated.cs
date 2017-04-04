namespace Orca_Gamma.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedimportantremovedexpertisvalidated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrivateMessages", "IsImportant", c => c.Boolean(nullable: false));
            DropColumn("dbo.Experts", "IsValidated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Experts", "IsValidated", c => c.Boolean(nullable: false));
            DropColumn("dbo.PrivateMessages", "IsImportant");
        }
    }
}
