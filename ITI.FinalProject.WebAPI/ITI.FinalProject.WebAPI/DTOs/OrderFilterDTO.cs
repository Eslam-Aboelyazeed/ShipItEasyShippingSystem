using Domain.Enums;

namespace ITI.FinalProject.WebAPI.DTOs
{
    public class OrderFilterDTO
    {
        public OrderStatus OrderStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
