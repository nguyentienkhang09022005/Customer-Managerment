namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class StaffResponse
    {
        public Guid IdStaff { get; set; }

        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
