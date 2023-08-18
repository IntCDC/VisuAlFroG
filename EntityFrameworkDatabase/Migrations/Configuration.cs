using EntityFrameworkDatabase.Models;
using System;
using System.Data.Entity.Migrations;
using Core.Utilities;



/*
 * Database Configuration
 * 
 */
namespace EntityFrameworkDatabase
{
    namespace Migrations
    {

        internal sealed class Configuration : DbMigrationsConfiguration<EntityFrameworkDatabase.Models.DatabaseContext>
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public Configuration()
            {
                // Set data directory variable given in connection string of DatabaseContext
                AppDomain.CurrentDomain.SetData("DataDirectory", WorkingDirectory.Path());

                AutomaticMigrationsEnabled = false;
            }

            protected override void Seed(EntityFrameworkDatabase.Models.DatabaseContext context)
            {
                //  This method will be called after migrating to the latest version.

                //  You can use the DbSet<T>.AddOrUpdate() helper extension method
                //  to avoid creating duplicate seed data.
                context.Entites.AddOrUpdate(
                    new Entity[] {
                    new Entity() { Id = 1, Title = "Title1", Name = "One" },
                    new Entity() { Id = 2, Title = "Title2", Name = "Two" },
                    new Entity() { Id = 3, Title = "Title3", Name = "Three" },
                    new Entity() { Id = 4, Title = "Title4", Name = "Four" }
                        }
                    );
            }
        }
    }
}
