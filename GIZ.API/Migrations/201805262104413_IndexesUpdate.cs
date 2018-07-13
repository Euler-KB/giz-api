namespace GIZ.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexesUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Phone", c => c.String(maxLength: 128));
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 256));
            CreateIndex("dbo.Users", "Username", unique: true, name: "IX_UsernameUniq");
            CreateIndex("dbo.Users", new[] { "Phone", "AccountType" }, unique: true, name: "IX_PhoneUniq");
            CreateIndex("dbo.Users", new[] { "Email", "AccountType" }, unique: true, name: "IX_EmailUniq");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "IX_EmailUniq");
            DropIndex("dbo.Users", "IX_PhoneUniq");
            DropIndex("dbo.Users", "IX_UsernameUniq");
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Phone", c => c.String());
        }
    }
}
