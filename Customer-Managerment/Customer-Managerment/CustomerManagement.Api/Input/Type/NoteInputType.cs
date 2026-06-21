using System.ComponentModel.DataAnnotations;
using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class NoteInput
    {
        [Required(ErrorMessage = "Content là bắt buộc.")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "Content phải từ 1-5000 ký tự.")]
        public string Content { get; set; } = null!;

        public NoteType Type { get; set; } = NoteType.COMMENT;

        [Required(ErrorMessage = "IdStaff là bắt buộc.")]
        public Guid IdStaff { get; set; }

        [Required(ErrorMessage = "LinkedEntityType là bắt buộc.")]
        [StringLength(50, ErrorMessage = "LinkedEntityType tối đa 50 ký tự.")]
        public string LinkedEntityType { get; set; } = null!;

        [Required(ErrorMessage = "LinkedEntityId là bắt buộc.")]
        public Guid LinkedEntityId { get; set; }

        public Guid? ParentNoteId { get; set; }
    }

    public class NoteUpdateInput
    {
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "Content phải từ 1-5000 ký tự.")]
        public string? Content { get; set; }

        public bool? IsPinned { get; set; }
    }
}