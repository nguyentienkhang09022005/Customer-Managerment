namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CustomerUpdateRequest
    {
        public PersonUpdateRequest Person { get; set; } = new();
    }
}
