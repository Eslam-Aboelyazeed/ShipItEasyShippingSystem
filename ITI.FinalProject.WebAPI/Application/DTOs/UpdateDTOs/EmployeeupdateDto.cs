using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UpdateDTOs
{
    public class EmployeeupdateDto
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }
}
