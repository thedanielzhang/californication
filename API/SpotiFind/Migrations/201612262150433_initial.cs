namespace SpotiFind.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using SpotiFind.Models;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlaceId = c.String(nullable: false),
                        PlaylistId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Locations");
        }


    }
}
