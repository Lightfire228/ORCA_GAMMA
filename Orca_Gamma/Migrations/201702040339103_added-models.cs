namespace Orca_Gamma.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedmodels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Catagories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Collaborators",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.UserId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectLead = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ProjectLead)
                .Index(t => t.ProjectLead);
            
            CreateTable(
                "dbo.Experts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsValidated = c.Boolean(nullable: false),
                        CatagoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Catagories", t => t.CatagoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.CatagoryId);
            
            CreateTable(
                "dbo.ForumThreads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Subject = c.String(),
                        FirstPost = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy, cascadeDelete: true)
                .Index(t => t.CreatedBy);
            
            CreateTable(
                "dbo.KeywordRelations",
                c => new
                    {
                        ExpertId = c.String(nullable: false, maxLength: 128),
                        KeywordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExpertId, t.KeywordId })
                .ForeignKey("dbo.Experts", t => t.ExpertId, cascadeDelete: true)
                .ForeignKey("dbo.Keywords", t => t.KeywordId, cascadeDelete: true)
                .Index(t => t.ExpertId)
                .Index(t => t.KeywordId);
            
            CreateTable(
                "dbo.Keywords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrivateMessagePosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.String(nullable: false, maxLength: 128),
                        PartOf = c.Int(nullable: false),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PrivateMessages", t => t.PartOf, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy, cascadeDelete: true)
                .Index(t => t.CreatedBy)
                .Index(t => t.PartOf);
            
            CreateTable(
                "dbo.PrivateMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Subject = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PrivateMessageBetweens",
                c => new
                    {
                        PrivateMessageId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.PrivateMessageId, t.UserId })
                .ForeignKey("dbo.PrivateMessages", t => t.PrivateMessageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.PrivateMessageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ThreadMessagePosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.String(nullable: false, maxLength: 128),
                        PartOf = c.Int(nullable: false),
                        Body = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumThreads", t => t.PartOf, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.PartOf);
            
            AddColumn("dbo.AspNetUsers", "IsDisabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThreadMessagePosts", "CreatedBy", "dbo.AspNetUsers");
            DropForeignKey("dbo.ThreadMessagePosts", "PartOf", "dbo.ForumThreads");
            DropForeignKey("dbo.PrivateMessageBetweens", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PrivateMessageBetweens", "PrivateMessageId", "dbo.PrivateMessages");
            DropForeignKey("dbo.PrivateMessagePosts", "CreatedBy", "dbo.AspNetUsers");
            DropForeignKey("dbo.PrivateMessagePosts", "PartOf", "dbo.PrivateMessages");
            DropForeignKey("dbo.PrivateMessages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.KeywordRelations", "KeywordId", "dbo.Keywords");
            DropForeignKey("dbo.KeywordRelations", "ExpertId", "dbo.Experts");
            DropForeignKey("dbo.ForumThreads", "CreatedBy", "dbo.AspNetUsers");
            DropForeignKey("dbo.Experts", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Experts", "CatagoryId", "dbo.Catagories");
            DropForeignKey("dbo.Collaborators", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Collaborators", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "ProjectLead", "dbo.AspNetUsers");
            DropIndex("dbo.ThreadMessagePosts", new[] { "PartOf" });
            DropIndex("dbo.ThreadMessagePosts", new[] { "CreatedBy" });
            DropIndex("dbo.PrivateMessageBetweens", new[] { "UserId" });
            DropIndex("dbo.PrivateMessageBetweens", new[] { "PrivateMessageId" });
            DropIndex("dbo.PrivateMessages", new[] { "UserId" });
            DropIndex("dbo.PrivateMessagePosts", new[] { "PartOf" });
            DropIndex("dbo.PrivateMessagePosts", new[] { "CreatedBy" });
            DropIndex("dbo.KeywordRelations", new[] { "KeywordId" });
            DropIndex("dbo.KeywordRelations", new[] { "ExpertId" });
            DropIndex("dbo.ForumThreads", new[] { "CreatedBy" });
            DropIndex("dbo.Experts", new[] { "CatagoryId" });
            DropIndex("dbo.Experts", new[] { "Id" });
            DropIndex("dbo.Projects", new[] { "ProjectLead" });
            DropIndex("dbo.Collaborators", new[] { "UserId" });
            DropIndex("dbo.Collaborators", new[] { "ProjectId" });
            DropColumn("dbo.AspNetUsers", "IsDisabled");
            DropTable("dbo.ThreadMessagePosts");
            DropTable("dbo.PrivateMessageBetweens");
            DropTable("dbo.PrivateMessages");
            DropTable("dbo.PrivateMessagePosts");
            DropTable("dbo.Keywords");
            DropTable("dbo.KeywordRelations");
            DropTable("dbo.ForumThreads");
            DropTable("dbo.Experts");
            DropTable("dbo.Projects");
            DropTable("dbo.Collaborators");
            DropTable("dbo.Catagories");
        }
    }
}
