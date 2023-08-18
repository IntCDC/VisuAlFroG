using System.Data.Entity;
using EntityFrameworkDatabase.Migrations;
using Core.Utilities;



/*
 * Database Context
 * 
 */
namespace EntityFrameworkDatabase
{
    namespace Models
    {

        public class DatabaseContext : DbContext
        {

            /* ------------------------------------------------------------------*/
            // public functions

            // You can add custom code to this file. Changes will not be overwritten.
            // 
            // If you want Entity Framework to drop and regenerate your database
            // automatically whenever you change your model schema, please use data migrations.
            // For more information refer to the documentation:
            // http://msdn.microsoft.com/en-us/data/jj591621.aspx


            /// <summary>
            /// Use base("name=DatabaseContext") to look for connectionString in App.config 
            /// => But App.config can not be used by Grasshopper component
            /// DataDirectory is defined in CTOR of Configuration
            /// </summary>
            public DatabaseContext() : base("Data Source=(localdb)\\MSSQLLocalDB; Integrated Security=True; MultipleActiveResultSets=True; AttachDbFilename=|DataDirectory|" + WorkingDirectory.FileName("database", "mdf"))
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>());
            }


            /* ------------------------------------------------------------------*/
            // public variables

            public System.Data.Entity.DbSet<EntityFrameworkDatabase.Models.Entity> Entites { get; set; }

        }
    }
}
