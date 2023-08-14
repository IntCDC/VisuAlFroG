using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/*
 * Database Model
 * 
 */
namespace EntityFrameworkDatabase
{
    namespace Models
    {
        public class Entity
        {

            /* ------------------------------------------------------------------*/
            // public variables

            public int Id { get; set; }
            [Required]


            public string Title { get; set; }


            public string Name { get; set; }

        }
    }
}
