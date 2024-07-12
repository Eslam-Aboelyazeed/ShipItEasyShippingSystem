using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RolePowers
    {
        
        [ForeignKey("ApplicationRoles")]
        public string RoleId { get; set; }
        public Tables TableName { get; set; }
        public bool Create { get; set; }
        public bool Delete { get; set; }
        public bool Update { get; set; }
        public bool Read { get; set; }
        public ApplicationRoles ApplicationRoles { get; set; }
    }

}
