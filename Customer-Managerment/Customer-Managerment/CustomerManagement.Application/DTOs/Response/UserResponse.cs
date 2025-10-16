namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class UserResponse
    {
        public Guid IdUser { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Fullname { get; set; }
    }
}
