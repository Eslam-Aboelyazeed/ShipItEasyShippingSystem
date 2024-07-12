using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class GovernorateDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public Status status { get; set; }
    }
}
