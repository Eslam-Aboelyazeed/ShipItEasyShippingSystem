using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public Status Status { get; set; }

        [ForeignKey("branch")]
        public int? BranchId { get; set; }
        public UserType UserType { get; set; }

        public Employee employee { get; set; }
        public Representative representative { get; set; }
        public Merchant merchant { get; set; }
        public Branch branch { get; set; }

    }
}
