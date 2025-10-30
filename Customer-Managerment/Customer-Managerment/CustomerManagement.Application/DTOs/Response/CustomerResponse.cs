namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class CustomerResponse
    {
        public Guid IdCustomer { get; set; }

        public DateTime? CreatedAt { get; set; }

        public PersonResponse? personResponse { get; set; }
    }
}
