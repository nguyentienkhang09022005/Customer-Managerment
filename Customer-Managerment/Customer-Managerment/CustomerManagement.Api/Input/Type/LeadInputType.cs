namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class LeadInput
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Resource { get; set; }
    }

    public class LeadUpdateInput
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Resource { get; set; }
    }
}