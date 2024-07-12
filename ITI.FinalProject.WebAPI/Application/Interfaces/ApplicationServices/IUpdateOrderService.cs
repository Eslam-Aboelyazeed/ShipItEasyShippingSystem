using Application.DTOs.DisplayDTOs;
using Application.DTOs.UpdateDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationServices
{
    public interface IUpdateOrderService
    {
        public Task<ModificationResultDTO> UpdateOrder(DeliveredOrderUpdateDTO orderDTO);
    }
}
