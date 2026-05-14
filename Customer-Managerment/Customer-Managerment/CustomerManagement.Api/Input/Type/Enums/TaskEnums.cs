namespace Customer_Managerment.CustomerManagement.Api.Input.Type.Enums
{
    public enum TaskPriority
    {
        LOW,
        MEDIUM,
        HIGH,
        URGENT
    }

    public enum TaskItemStatus
    {
        PENDING,
        IN_PROGRESS,
        COMPLETED,
        CANCELLED
    }

    public enum TaskLinkedEntity
    {
        Lead,
        Customer,
        Deal
    }
}