namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class OrderCreationRequest
    {
        public DateOnly? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }

        public Guid IdUser { get; set; }

        public Guid IdCustomer { get; set; }
    }
}
