using EntityFrameworkDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Utilities;
using Core.Management;



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

            _timer.Stop(this.GetType().FullName);
            _initilized = true;
            return _initilized;
        }


        public override bool Execute()
        {
            if (!_initilized)
            {
                return false;
            }
            _timer.Start();

            IQueryable<Entity> query = _database_context.Entites.Where(e => e.Id == 4);
            foreach (Entity obj in query)
            {
                Log.Default.Msg(Log.Level.Info, obj.Title);
            }

            _timer.Stop(this.GetType().FullName);
            return true;
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private DatabaseContext _database_context;

    }
}
