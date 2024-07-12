using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class RolePowersDTO
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime TimeOfAddtion { get; set; }
        public List<PowersDTO>? Powers { get; set; }
    }
}
