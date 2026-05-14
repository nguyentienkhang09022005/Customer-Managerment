using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class TaskInput
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.LOW;
        public TaskItemStatus Status { get; set; } = TaskItemStatus.PENDING;
        public Guid IdStaffAssigned { get; set; }
        public string? LinkedEntityType { get; set; }
        public Guid? LinkedEntityId { get; set; }
    }

    public class TaskUpdateInput
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority? Priority { get; set; }
        public TaskItemStatus? Status { get; set; }
        public Guid? IdStaffAssigned { get; set; }
        public string? LinkedEntityType { get; set; }
        public Guid? LinkedEntityId { get; set; }
    }
}