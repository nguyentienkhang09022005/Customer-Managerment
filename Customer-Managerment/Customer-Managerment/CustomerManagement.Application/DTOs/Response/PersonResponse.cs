namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class PersonResponse
    {
        public Guid Id { get; set; }

        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}