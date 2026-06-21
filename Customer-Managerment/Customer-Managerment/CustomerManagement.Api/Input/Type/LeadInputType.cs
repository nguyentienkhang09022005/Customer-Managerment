using System.ComponentModel.DataAnnotations;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class LeadInput
    {
        [Required(ErrorMessage = "Fullname là bắt buộc.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Fullname phải từ 2-100 ký tự.")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "Location tối đa 200 ký tự.")]
        public string? Location { get; set; }

        [StringLength(100, ErrorMessage = "Resource tối đa 100 ký tự.")]
        public string? Resource { get; set; }
    }

    public class LeadUpdateInput
    {
        [Required(ErrorMessage = "Fullname là bắt buộc.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Fullname phải từ 2-100 ký tự.")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "Location tối đa 200 ký tự.")]
        public string? Location { get; set; }

        [StringLength(100, ErrorMessage = "Resource tối đa 100 ký tự.")]
        public string? Resource { get; set; }
    }
}