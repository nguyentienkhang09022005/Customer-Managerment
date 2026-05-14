namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class TaskPriorityConstant
    {
        public const string TaskPriorityLow = "LOW";
        public const string TaskPriorityMedium = "MEDIUM";
        public const string TaskPriorityHigh = "HIGH";
        public const string TaskPriorityUrgent = "URGENT";

        public static readonly string[] Priorities = {
            TaskPriorityLow,
            TaskPriorityMedium,
            TaskPriorityHigh,
            TaskPriorityUrgent
        };

        public static int FromString(string priority)
        {
            return priority.ToUpper() switch
            {
                "LOW" => 0,
                "MEDIUM" => 1,
                "HIGH" => 2,
                "URGENT" => 3,
                _ => 0
            };
        }

        public static string ToString(int priority)
        {
            return priority switch
            {
                0 => TaskPriorityLow,
                1 => TaskPriorityMedium,
                2 => TaskPriorityHigh,
                3 => TaskPriorityUrgent,
                _ => TaskPriorityLow
            };
        }
    }
}