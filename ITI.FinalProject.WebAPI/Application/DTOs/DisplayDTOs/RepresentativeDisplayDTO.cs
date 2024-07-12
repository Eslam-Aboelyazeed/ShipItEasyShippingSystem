using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class RepresentativeDisplayDTO
    {
        public string Id { get; set; }
        public DeductionType DiscountType { get; set; }
        public double CompanyPercentage { get; set; }
        public string UserFullName { get; set; }
        public string Email { get; set; }
        public string UserAddress { get; set; }
        public string UserPhoneNo { get; set; }
        public Status UserStatus { get; set; }
        public int UserBranchId { get; set; }
        public UserType UserType { get; set; }
        public List<int> GovernorateIds { get; set; }
    }
}
