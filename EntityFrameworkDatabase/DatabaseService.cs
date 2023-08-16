﻿using EntityFrameworkDatabase.Models;
using System.Linq;
using Core.Utilities;
using Core.Abstracts;



/*
 * Entity Framework Database
 * 
 */
namespace EntityFrameworkDatabase
{
    public class DatabaseService : AbstractService
    {

        /* ------------------------------------------------------------------*/
        // public functions

        public override bool Initialize()
        {
            _timer.Start();

            _database_context = new DatabaseContext();

            _timer.Stop();
            _initilized = true;
            return _initilized;
        }


        public override bool Execute()
        {
            if (!_initilized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }
            _timer.Start();

            IQueryable<Entity> query = _database_context.Entites.Where(e => e.Id == 4);
            foreach (Entity obj in query)
            {
                Log.Default.Msg(Log.Level.Info, obj.Title);
            }

            _timer.Stop();
            return true;
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private DatabaseContext _database_context;

    }
}