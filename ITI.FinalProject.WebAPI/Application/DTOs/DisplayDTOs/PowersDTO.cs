using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class PowersDTO
    {
        public Tables TableName { get; set; }
        public bool Create { get; set; }
        public bool Delete { get; set; }
        public bool Update { get; set; }
        public bool Read { get; set; }
    }
}
