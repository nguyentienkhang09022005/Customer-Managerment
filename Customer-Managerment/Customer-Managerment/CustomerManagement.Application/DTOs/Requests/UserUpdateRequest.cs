namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class UserUpdateRequest
    {
        public string? Fullname { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Role { get; set; }
    }
}
