using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationRoles:IdentityRole<string>
    {
        public DateTime TimeOfAddition { get; set; }
        public List<RolePowers> RolePowers { get; set; }
    }
}
