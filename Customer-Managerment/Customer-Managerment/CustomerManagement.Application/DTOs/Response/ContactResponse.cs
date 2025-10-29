namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class ContactResponse
    {
        public Guid IdContact { get; set; }

        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public LeadResponse? infLeadResponse { get; set; }

        public StaffResponse? infStaffResponse { get; set; }
    } 
}
