namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class LeadCreationRequest
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Resource { get; set; }
    }

    public class LeadUpdateRequest
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Resource { get; set; }
    }
}