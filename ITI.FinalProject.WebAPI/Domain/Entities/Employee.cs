using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Employee
    {
        [ForeignKey("user")]
        [Key]
        public string userId { get; set; }

        public ApplicationUser user { get; set; }
    }
}
