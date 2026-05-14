using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class NoteInput
    {
        public string Content { get; set; } = null!;
        public NoteType Type { get; set; } = NoteType.COMMENT;
        public Guid IdStaff { get; set; }
        public string LinkedEntityType { get; set; } = null!;
        public Guid LinkedEntityId { get; set; }
        public Guid? ParentNoteId { get; set; }
    }

    public class NoteUpdateInput
    {
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
    }
}