using System.ComponentModel.DataAnnotations;



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
