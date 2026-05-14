namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class LeadResponse
    {
        public Guid Id { get; set; }

        public PersonResponse Person { get; set; } = null!;

        public string? Resource { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}