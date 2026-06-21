using System.ComponentModel.DataAnnotations;
using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class ContactInput
    {
        [Required(ErrorMessage = "Type là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Type tối đa 50 ký tự.")]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "Title là bắt buộc.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Content tối đa 2000 ký tự.")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "IdStaff là bắt buộc.")]
        public Guid IdStaff { get; set; }

        [Required(ErrorMessage = "IdLead là bắt buộc.")]
        public Guid IdLead { get; set; }
    }

    public class ContactUpdateInput
    {
        [StringLength(50, ErrorMessage = "Type tối đa 50 ký tự.")]
        public string? Type { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Content tối đa 2000 ký tự.")]
        public string? Content { get; set; }

        public ContactStatus? Status { get; set; }
    }
}