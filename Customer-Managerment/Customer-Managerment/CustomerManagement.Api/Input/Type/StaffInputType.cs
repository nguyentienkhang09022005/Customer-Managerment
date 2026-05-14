using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class StaffInput
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public StaffRole Role { get; set; }
        public decimal? Salary { get; set; }
    }

    public class StaffUpdateInput
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public StaffRole? Role { get; set; }
        public decimal? Salary { get; set; }
    }
}