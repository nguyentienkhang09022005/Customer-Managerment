namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }

        public PersonResponse Person { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}