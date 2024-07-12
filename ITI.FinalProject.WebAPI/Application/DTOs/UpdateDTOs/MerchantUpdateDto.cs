using Application.DTOs.InsertDTOs;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UpdateDTOs
{
    public class MerchantUpdateDto
    {
        public string Id { get; set; }
        public Status Status { get; set; }
    }
}
