using System.ComponentModel.DataAnnotations;
using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class StaffInput
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

        [Required(ErrorMessage = "Username là bắt buộc.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải từ 3-50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username chỉ được chứa chữ, số, '.', '_', '-'.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password là bắt buộc.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password phải từ 8-100 ký tự.")]
        public string Password { get; set; } = null!;

        public StaffRole Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary phải >= 0.")]
        public decimal? Salary { get; set; }
    }

    public class StaffUpdateInput
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

        public StaffRole? Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary phải >= 0.")]
        public decimal? Salary { get; set; }
    }
}