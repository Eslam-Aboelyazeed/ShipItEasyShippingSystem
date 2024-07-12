using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class MerchantResponseDto
    {
        public string Id { get; set; }
        public string? StoreName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string BranchName { get; set; }
        public string CityName { get; set; }
        public string GovernorateName { get; set; }
        public Status Status { get; set; }
        public decimal MerchantPayingPercentageForRejectedOrders { get; set; }
        public decimal? SpecialPickupShippingCost { get; set; }
        public List<SpecialPackageDTO> SpecialPackages { get; set; }
        public List<DisplayOrderDTO> orders { get; set; }
    }
}
