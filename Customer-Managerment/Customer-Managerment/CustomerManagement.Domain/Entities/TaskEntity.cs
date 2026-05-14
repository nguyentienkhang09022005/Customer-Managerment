namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class TaskEntity
    {
        public Guid IdTask { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Foreign keys
        public Guid IdStaffAssigned { get; set; }

        // Linked entity (Lead, Customer, Deal)
        public string? LinkedEntityType { get; set; }
        public Guid? LinkedEntityId { get; set; }

        // Navigation property
        public virtual Person? IdStaffAssignedNavigation { get; set; }
    }
}