using System.ComponentModel.DataAnnotations;
using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class DealInput
    {
        [Required(ErrorMessage = "Title là bắt buộc.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Content tối đa 2000 ký tự.")]
        public string? Content { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price phải >= 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "IdStaff là bắt buộc.")]
        public Guid IdStaff { get; set; }

        [Required(ErrorMessage = "IdCustomer là bắt buộc.")]
        public Guid IdCustomer { get; set; }
    }

    public class DealUpdateInput
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Content tối đa 2000 ký tự.")]
        public string? Content { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price phải >= 0.")]
        public decimal? Price { get; set; }

        public DealStatus? Status { get; set; }
    }
}