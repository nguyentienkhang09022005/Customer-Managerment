namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ProductUpdateRequest
    {
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Category { get; set; }

        public string? Status { get; set; }
    }
}
