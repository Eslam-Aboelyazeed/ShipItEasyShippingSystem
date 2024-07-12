using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class DisplayOrderDTO
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public string Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalWeight { get; set; }
        public string VillageAndStreet { get; set; }
        public bool ShippingToVillage { get; set; }
        public string MerchantName { get; set; }
        public string GovernorateName { get; set; }
        public string CityName { get; set; }
        public string BranchName { get; set; }
        public ShippingTypes ShippingType { get; set; }
        public string? RepresentativeName { get; set; }
        public decimal? OrderMoneyReceived { get; set; }
        public decimal? ShippingMoneyReceived { get; set; }
        public decimal? CompanyProfit { get; set; }
        public decimal ShippingCost { get; set; }

        public OrderStatus Status { get; set; }
        public OrderTypes Type { get; set; }
        public PaymentTypes PaymentType { get; set; }

        public List<DisplayProductDTO> Products { get; set; }
    }
}
