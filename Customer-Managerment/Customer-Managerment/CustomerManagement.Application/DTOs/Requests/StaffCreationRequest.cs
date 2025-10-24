namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class StaffCreationRequest
    {
        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Role { get; set; }
    }
}
