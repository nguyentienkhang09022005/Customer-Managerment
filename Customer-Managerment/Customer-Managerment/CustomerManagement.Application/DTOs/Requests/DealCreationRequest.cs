namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class DealCreationRequest
    {
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public decimal? Price { get; set; }
        public Guid IdStaff { get; set; }
        public Guid IdCustomer { get; set; }
    }

    public class DealUpdateRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public decimal? Price { get; set; }
        public string? Status { get; set; }
    }
}