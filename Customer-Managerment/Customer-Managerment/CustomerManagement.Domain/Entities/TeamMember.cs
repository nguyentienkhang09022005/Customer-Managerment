namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class TeamMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public Guid IdStaff { get; set; }
        public int Role { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public string? AssignedBy { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        public virtual Person? IdStaffNavigation { get; set; }
    }
}