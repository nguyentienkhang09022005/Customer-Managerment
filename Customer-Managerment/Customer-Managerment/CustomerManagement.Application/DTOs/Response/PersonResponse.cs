namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class PersonResponse
    {
        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public decimal? Salary { get; set; }

        public string? Location { get; set; }
    }
}
