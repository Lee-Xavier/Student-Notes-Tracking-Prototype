namespace HelpTrackingv1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Notes", new[] { "Student_Id" });
            AlterColumn("dbo.Notes", "Student_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Notes", "Student_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Notes", new[] { "Student_Id" });
            AlterColumn("dbo.Notes", "Student_Id", c => c.Int());
            CreateIndex("dbo.Notes", "Student_Id");
        }
    }
}
