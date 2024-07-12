using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InsertDTOs
{
    public class InsertOrderDTO
    {
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public string VillageAndStreet { get; set; }
        public bool ShippingToVillage { get; set; }
        public decimal? OrderMoneyReceived { get; set; }
        public decimal? ShippingMoneyReceived { get; set; }
        public ShippingTypes ShippingType { get; set; }
        public string MerchantId { get; set; }
        public int GovernorateId { get; set; }
        public int CityId { get; set; }
        public int BranchId { get; set; }
        public string? RepresentativeId { get; set; }
        public OrderStatus Status { get; set; }
        public OrderTypes Type { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public List<InsertProductDTO> Products { get; set; }
    }
}
