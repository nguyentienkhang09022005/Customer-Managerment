namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class OrderResponse
    {
        public Guid IdOrder { get; set; }

        public DateOnly? OrderDate { get; set; }

        public string? Status { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }

        public Guid? IdUser { get; set; }

        public Guid? IdCustomer { get; set; }
    }
}
