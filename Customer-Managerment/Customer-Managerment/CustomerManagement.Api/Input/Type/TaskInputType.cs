using System.ComponentModel.DataAnnotations;
using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class TaskInput
    {
        [Required(ErrorMessage = "Title là bắt buộc.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Description tối đa 2000 ký tự.")]
        public string? Description { get; set; }

        public string? DueDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.LOW;

        public TaskItemStatus Status { get; set; } = TaskItemStatus.PENDING;

        [Required(ErrorMessage = "IdStaffAssigned là bắt buộc.")]
        public Guid IdStaffAssigned { get; set; }

        [StringLength(50, ErrorMessage = "LinkedEntityType tối đa 50 ký tự.")]
        public string? LinkedEntityType { get; set; }

        public Guid? LinkedEntityId { get; set; }
    }

    public class TaskUpdateInput
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description tối đa 2000 ký tự.")]
        public string? Description { get; set; }

        public string? DueDate { get; set; }

        public TaskPriority? Priority { get; set; }

        public TaskItemStatus? Status { get; set; }

        public Guid? IdStaffAssigned { get; set; }

        [StringLength(50, ErrorMessage = "LinkedEntityType tối đa 50 ký tự.")]
        public string? LinkedEntityType { get; set; }

        public Guid? LinkedEntityId { get; set; }
    }
}