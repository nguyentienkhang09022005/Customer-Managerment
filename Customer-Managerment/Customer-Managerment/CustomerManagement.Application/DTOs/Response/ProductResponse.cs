namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class ProductResponse
    {
        public Guid IdProduct { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Category { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
