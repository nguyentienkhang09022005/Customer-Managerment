namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class StaffResponse
    {
        public Guid Id { get; set; }

        public PersonResponse Person { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string? Role { get; set; }

        public decimal? Salary { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}