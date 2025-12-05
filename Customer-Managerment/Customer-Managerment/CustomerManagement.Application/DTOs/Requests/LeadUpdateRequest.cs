namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class LeadUpdateRequest
    {
        public string? Resource { get; set; }

        public PersonUpdateRequest Person { get; set; } = new();
    }

    public class PersonUpdateRequest
    {
        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public decimal? Salary { get; set; }

        public string? Location { get; set; }
    }
}
