namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class TeamMemberResponse
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public Guid IdStaff { get; set; }
        public StaffResponse? Staff { get; set; }
        public string Role { get; set; } = null!;
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}