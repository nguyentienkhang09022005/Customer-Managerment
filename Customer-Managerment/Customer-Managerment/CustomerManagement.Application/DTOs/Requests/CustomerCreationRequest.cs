namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CustomerCreationRequest
    {
        public PersonCreationRequest Person { get; set; } = new();
    }
}
