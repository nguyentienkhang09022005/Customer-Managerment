namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class ContactResponse
    {
        public Guid IdContact { get; set; }

        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public LeadResponse? Lead { get; set; }

        public StaffResponse? Staff { get; set; }
    }
}