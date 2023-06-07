using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkDatabase.Models
{
    public class Entity
    {
        public int Id { get; set; }
        [Required]

        public string Title { get; set; }

        public string Name { get; set; }

    }
}
