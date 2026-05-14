namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class NoteResponse
    {
        public Guid IdNote { get; set; }
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsPinned { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid IdStaff { get; set; }
        public StaffResponse? Staff { get; set; }
        public string LinkedEntityType { get; set; } = null!;
        public Guid LinkedEntityId { get; set; }
        public Guid? ParentNoteId { get; set; }
        public List<NoteResponse>? Replies { get; set; }
        public List<NoteMentionResponse>? Mentions { get; set; }
    }

    public class NoteMentionResponse
    {
        public Guid IdMention { get; set; }
        public Guid IdNote { get; set; }
        public Guid IdStaffMentioned { get; set; }
        public StaffResponse? StaffMentioned { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}