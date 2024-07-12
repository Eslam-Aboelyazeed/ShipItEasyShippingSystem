using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class ModificationResultDTO
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
    }
}
