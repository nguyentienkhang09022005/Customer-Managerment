namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class DealResponse
    {
        public Guid IdDeal { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public decimal? Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Status { get; set; }

        public CustomerResponse? Customer { get; set; }

        public StaffResponse? Staff { get; set; }
    }
}