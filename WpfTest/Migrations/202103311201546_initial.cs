namespace WpfTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.States",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        State_value = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(),
                        StateId = c.Int(nullable: false),
                        UserId = c.Int(),
                        Name = c.String(),
                        Description = c.String(),
                        List_exe = c.String(),
                        Planned_labor_intensity = c.Single(nullable: false),
                        Date_cteate = c.DateTimeOffset(nullable: false, precision: 7),
                        Date_update = c.DateTimeOffset(nullable: false, precision: 7),
                        Date_start = c.DateTimeOffset(nullable: false, precision: 7),
                        Date_finish = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Tasks");
            DropTable("dbo.States");
        }
    }
}
