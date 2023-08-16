using System;
using System.Data.Entity.Migrations;



/*
 * Database Migration
 * 
 */
namespace EntityFrameworkDatabase
{
    namespace Migrations
    { 
        public partial class Initial : DbMigration
        {
            public override void Up()
            {
                CreateTable(
                    "dbo.Entities",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Name = c.String(),
                    })
                    .PrimaryKey(t => t.Id);

            }

            public override void Down()
            {
                DropTable("dbo.Entities");
            }
        }
    }
}
