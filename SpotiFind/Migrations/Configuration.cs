namespace SpotiFind.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SpotiFind.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SpotiFind.Models.SpotiFindContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SpotiFind.Models.SpotiFindContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Locations.AddOrUpdate(
                p => p.Id,
                new Location() { Id = 0, Place = "ChIJC4xmE1LdMIgRyQsQaH9Aawk", Playlist = "5TJ9mC3q0XG5uLzQTkCeo2" }
                );
        }
    }
}
