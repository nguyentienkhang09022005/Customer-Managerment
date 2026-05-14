namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class Contact
    {
        public Guid IdContact { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Foreign keys
        public Guid IdStaff { get; set; }
        public Guid IdLead { get; set; }

        // Navigation properties
        public virtual Person? IdStaffNavigation { get; set; }
        public virtual Person? IdLeadNavigation { get; set; }
    }
}