namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class StaffStatusResponse
    {
        public Guid IdStaff { get; set; }
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Status { get; set; }
        public string StatusName { get; set; } = null!;
        public DateTime? LastActiveAt { get; set; }
    }
}