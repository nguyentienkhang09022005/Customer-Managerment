namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class OrderUpdateRequest
    {
        public string? Status { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
