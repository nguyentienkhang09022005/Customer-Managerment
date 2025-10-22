namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class OrderCreationRequest
    {
        public string? PaymentMethod { get; set; }

        public Guid IdCustomer { get; set; }

        public List<OrderDetailRequest> orderDetailRequests { get; set; } = new();
    }

    public class OrderDetailRequest
    {
        public Guid IdProduct { get; set; }
        public int Quantity { get; set; }
    }
}
