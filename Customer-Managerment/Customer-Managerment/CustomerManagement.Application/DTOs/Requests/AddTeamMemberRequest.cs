namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class AddTeamMemberRequest
    {
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public Guid IdStaff { get; set; }
        public string Role { get; set; } = "MEMBER";
        public string? AssignedBy { get; set; }
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }
}