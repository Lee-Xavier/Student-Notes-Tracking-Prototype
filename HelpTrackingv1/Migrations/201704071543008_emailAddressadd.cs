namespace HelpTrackingv1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailAddressadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "EmailAddress", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Students", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "Email", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Students", "EmailAddress");
        }
    }
}
