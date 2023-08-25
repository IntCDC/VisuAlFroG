using EntityFrameworkDatabase.Models;
using System.Linq;
using Core.Utilities;
using Core.Abstracts;



/*
 * Entity Framework Database
 * 
 */
/* TEST
Execute:
    IQueryable<Entity> query = _database_context.Entites.Where(e => e.Id == 4);
    foreach (Entity obj in query)
    {
        Log.Default.Msg(Log.Level.Debug, obj.Title);
    }
*/
namespace EntityFrameworkDatabase
{
    public class DatabaseService : AbstractService
    {

        /* ------------------------------------------------------------------*/
        // public functions

        public override bool Initialize()
        {
            if (_initilized)
            {
                Terminate();
            }
            _timer.Start();

            _database_context = new DatabaseContext();

            _timer.Stop();
            _initilized = true;
            if (_initilized)
            {
                Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
            }
            return _initilized;
        }


        public override bool Terminate()
        {
            if (_initilized)
            {
                _database_context.Dispose();
                _database_context = null;

                _initilized = false;
            }
            return true;
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private DatabaseContext _database_context = null;

    }
}
