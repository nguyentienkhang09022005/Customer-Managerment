namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class NoteMention
    {
        public Guid IdMention { get; set; } = Guid.NewGuid();
        public Guid IdNote { get; set; }
        public Guid IdStaffMentioned { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Note? IdNoteNavigation { get; set; }
        public virtual Person? IdStaffMentionedNavigation { get; set; }
    }
}