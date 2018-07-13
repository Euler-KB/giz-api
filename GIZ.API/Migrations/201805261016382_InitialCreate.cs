namespace GIZ.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Region = c.String(),
                        Country = c.String(),
                        Location = c.String(),
                        Comment = c.String(),
                        LocationLat = c.Decimal(precision: 18, scale: 2),
                        LocationLng = c.Decimal(precision: 18, scale: 2),
                        Phone = c.String(),
                        Email = c.String(),
                        EstablishmentDate = c.DateTime(),
                        Dealer_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Dealer_Id, cascadeDelete: true)
                .Index(t => t.Dealer_Id);
            
            CreateTable(
                "dbo.CompanyBranches",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Region = c.String(),
                        Country = c.String(),
                        Location = c.String(),
                        LocationLat = c.Decimal(precision: 18, scale: 2),
                        LocationLng = c.Decimal(precision: 18, scale: 2),
                        Comment = c.String(),
                        Phone = c.String(),
                        EstablishmentDate = c.DateTime(),
                        Company_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id, cascadeDelete: true)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Username = c.String(nullable: false, maxLength: 256),
                        Phone = c.String(),
                        IsPhoneConfirmed = c.Boolean(nullable: false),
                        Email = c.String(),
                        IsEmailConfirmed = c.Boolean(nullable: false),
                        PasswordSalt = c.String(nullable: false),
                        PasswordHash = c.String(nullable: false),
                        AccountType = c.Int(nullable: false),
                        FullName = c.String(nullable: false),
                        LastLogin = c.DateTime(),
                        LastAccessTime = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProductType = c.String(nullable: false),
                        Classification = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Comments = c.String(),
                        Brand = c.String(),
                        CropType = c.String(),
                        DealerId = c.Long(nullable: false),
                        DateDeleted = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.DealerId, cascadeDelete: true)
                .Index(t => t.DealerId);
            
            CreateTable(
                "dbo.ProductLifeCycles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        Product_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OriginalPath = c.String(nullable: false, maxLength: 256),
                        ThumbnailPath = c.String(maxLength: 256),
                        Tag = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        ProductLifeCycle_Id = c.Long(),
                        Product_Id = c.Long(),
                        AppUser_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductLifeCycles", t => t.ProductLifeCycle_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.Users", t => t.AppUser_Id)
                .Index(t => t.ProductLifeCycle_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.LifeCycleTags",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LifeCycleId = c.Long(nullable: false),
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductLifeCycles", t => t.LifeCycleId, cascadeDelete: true)
                .Index(t => new { t.Key, t.LifeCycleId }, unique: true, name: "IX_KeyLifeCylce");
            
            CreateTable(
                "dbo.ProductProperties",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProductId = c.Long(nullable: false),
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.Key, t.ProductId }, unique: true, name: "IX_ProductPropUniq");
            
            CreateTable(
                "dbo.Solutions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Statement = c.String(nullable: false),
                        Product_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.ProductViews",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(),
                        ProductId = c.Long(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.UserTokens",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Salt = c.String(nullable: false),
                        Token = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        User_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTokens", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Media", "AppUser_Id", "dbo.Users");
            DropForeignKey("dbo.Products", "DealerId", "dbo.Users");
            DropForeignKey("dbo.ProductViews", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductViews", "UserId", "dbo.Users");
            DropForeignKey("dbo.Solutions", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductProperties", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Media", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductLifeCycles", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.LifeCycleTags", "LifeCycleId", "dbo.ProductLifeCycles");
            DropForeignKey("dbo.Media", "ProductLifeCycle_Id", "dbo.ProductLifeCycles");
            DropForeignKey("dbo.Companies", "Dealer_Id", "dbo.Users");
            DropForeignKey("dbo.CompanyBranches", "Company_Id", "dbo.Companies");
            DropIndex("dbo.UserTokens", new[] { "User_Id" });
            DropIndex("dbo.ProductViews", new[] { "ProductId" });
            DropIndex("dbo.ProductViews", new[] { "UserId" });
            DropIndex("dbo.Solutions", new[] { "Product_Id" });
            DropIndex("dbo.ProductProperties", "IX_ProductPropUniq");
            DropIndex("dbo.LifeCycleTags", "IX_KeyLifeCylce");
            DropIndex("dbo.Media", new[] { "AppUser_Id" });
            DropIndex("dbo.Media", new[] { "Product_Id" });
            DropIndex("dbo.Media", new[] { "ProductLifeCycle_Id" });
            DropIndex("dbo.ProductLifeCycles", new[] { "Product_Id" });
            DropIndex("dbo.Products", new[] { "DealerId" });
            DropIndex("dbo.CompanyBranches", new[] { "Company_Id" });
            DropIndex("dbo.Companies", new[] { "Dealer_Id" });
            DropTable("dbo.UserTokens");
            DropTable("dbo.ProductViews");
            DropTable("dbo.Solutions");
            DropTable("dbo.ProductProperties");
            DropTable("dbo.LifeCycleTags");
            DropTable("dbo.Media");
            DropTable("dbo.ProductLifeCycles");
            DropTable("dbo.Products");
            DropTable("dbo.Users");
            DropTable("dbo.CompanyBranches");
            DropTable("dbo.Companies");
        }
    }
}
