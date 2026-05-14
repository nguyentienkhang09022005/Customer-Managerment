namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class StaffUpdateRequest
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Role { get; set; }
        public decimal? Salary { get; set; }
    }
}