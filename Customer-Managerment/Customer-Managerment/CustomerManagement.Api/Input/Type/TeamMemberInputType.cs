namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class AddTeamMemberInput
    {
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public Guid IdStaff { get; set; }
        public Enums.TeamRole Role { get; set; } = Enums.TeamRole.MEMBER;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

    public class UpdateTeamMemberInput
    {
        public Enums.TeamRole? Role { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanDelete { get; set; }
    }
}