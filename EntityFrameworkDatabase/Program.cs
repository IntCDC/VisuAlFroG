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
    internal class Program
    {

        private static DatabaseContext db = new DatabaseContext();

        static void Main(string[] args)
        {

            
            IQueryable<Entity> query = db.Entites.Where(e => e.Id == 4);
            foreach (var obj in query)
            {
                Console.WriteLine(obj.Title);
            }
            
            Console.WriteLine("\nPress Enter to quit.");
            Console.ReadLine();

        }
    }
}
