﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using EntityFrameworkDatabase.Migrations;
using System.Runtime.Remoting.Contexts;


namespace EntityFrameworkDatabase.Models
{

    public class DatabaseContext : DbContext
    {

        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public DatabaseContext() : base("name=DatabaseContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>());
        }

        public System.Data.Entity.DbSet<EntityFrameworkDatabase.Models.Entity> Entites { get; set; }



    }
}

