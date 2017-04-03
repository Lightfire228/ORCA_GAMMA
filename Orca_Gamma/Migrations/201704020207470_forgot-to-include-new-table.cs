namespace Orca_Gamma.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forgottoincludenewtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThreadKeywords",
                c => new
                    {
                        KeywordId = c.Int(nullable: false),
                        ThreadId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.KeywordId, t.ThreadId })
                .ForeignKey("dbo.Keywords", t => t.KeywordId, cascadeDelete: true)
                .ForeignKey("dbo.ForumThreads", t => t.ThreadId, cascadeDelete: true)
                .Index(t => t.KeywordId)
                .Index(t => t.ThreadId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThreadKeywords", "ThreadId", "dbo.ForumThreads");
            DropForeignKey("dbo.ThreadKeywords", "KeywordId", "dbo.Keywords");
            DropIndex("dbo.ThreadKeywords", new[] { "ThreadId" });
            DropIndex("dbo.ThreadKeywords", new[] { "KeywordId" });
            DropTable("dbo.ThreadKeywords");
        }
    }
}
