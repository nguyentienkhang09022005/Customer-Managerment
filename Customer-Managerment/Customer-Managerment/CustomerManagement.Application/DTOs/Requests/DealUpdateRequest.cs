namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class DealUpdateRequest
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public decimal? Price { get; set; }

        public string? Status { get; set; }
    }
}
