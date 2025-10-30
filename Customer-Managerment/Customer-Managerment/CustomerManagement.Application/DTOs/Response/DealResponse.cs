namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class DealResponse
    {
        public Guid IdDeal { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Status { get; set; }

        public CustomerResponse? infCustomerResponse { get; set; }

        public StaffResponse? infStaffResponse { get; set; }
    }
}
