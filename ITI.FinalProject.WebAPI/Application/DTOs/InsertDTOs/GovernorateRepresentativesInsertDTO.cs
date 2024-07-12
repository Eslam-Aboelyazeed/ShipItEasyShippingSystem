using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InsertDTOs
{
    public class GovernorateRepresentativesInsertDTO
    {
        public int governorateId { get; set; }
        public string representativeId { get; set; }
    }
}
