namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ProductCreationRequest
    {
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Category { get; set; }
    }
}
