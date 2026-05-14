namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class Note
    {
        public Guid IdNote { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsPinned { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Foreign key - người tạo
        public Guid IdStaff { get; set; }

        // Linked entity info
        public string LinkedEntityType { get; set; } = null!;
        public Guid LinkedEntityId { get; set; }

        // Reply threading
        public Guid? ParentNoteId { get; set; }

        // Navigation properties
        public virtual Person? IdStaffNavigation { get; set; }
        public virtual Note? ParentNote { get; set; }
        public virtual ICollection<Note> Replies { get; set; } = new List<Note>();
        public virtual ICollection<NoteMention> Mentions { get; set; } = new List<NoteMention>();
    }
}