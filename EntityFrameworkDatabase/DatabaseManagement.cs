using EntityFrameworkDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



/*
 * Entity Framework Database
 * 
 */
namespace EntityFrameworkDatabase
{
    public class DatabaseManagement
    {

        private DatabaseContext db;


        public void Initialize()
        {


            this.db = new DatabaseContext();

            IQueryable<Entity> query = db.Entites.Where(e => e.Id == 4);
            foreach (Entity obj in query)
            {
                Console.WriteLine(obj.Title);
            }
            
            Console.WriteLine("\nPress Enter to quit.");
            string input = Console.ReadLine();

        }
    }
}
