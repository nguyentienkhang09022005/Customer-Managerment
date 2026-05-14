namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class TaskStatusConstant
    {
        public const string TaskStatusPending = "PENDING";
        public const string TaskStatusInProgress = "IN_PROGRESS";
        public const string TaskStatusCompleted = "COMPLETED";
        public const string TaskStatusCancelled = "CANCELLED";

        public static readonly string[] Statuses = {
            TaskStatusPending,
            TaskStatusInProgress,
            TaskStatusCompleted,
            TaskStatusCancelled
        };

        public static int FromString(string status)
        {
            return status.ToUpper() switch
            {
                "PENDING" => 0,
                "IN_PROGRESS" => 1,
                "COMPLETED" => 2,
                "CANCELLED" => 3,
                _ => 0
            };
        }

        public static string ToString(int status)
        {
            return status switch
            {
                0 => TaskStatusPending,
                1 => TaskStatusInProgress,
                2 => TaskStatusCompleted,
                3 => TaskStatusCancelled,
                _ => TaskStatusPending
            };
        }
    }
}