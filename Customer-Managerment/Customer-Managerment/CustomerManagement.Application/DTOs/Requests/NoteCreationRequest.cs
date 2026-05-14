namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class NoteCreationRequest
    {
        public string Content { get; set; } = null!;
        public string Type { get; set; } = "COMMENT";
        public Guid IdStaff { get; set; }
        public string LinkedEntityType { get; set; } = null!;
        public Guid LinkedEntityId { get; set; }
        public Guid? ParentNoteId { get; set; }
    }

    public class NoteUpdateRequest
    {
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
    }
}