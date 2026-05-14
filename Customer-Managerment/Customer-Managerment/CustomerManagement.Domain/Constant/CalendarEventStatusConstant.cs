namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class CalendarEventStatusConstant
    {
        public const string EventStatusScheduled = "SCHEDULED";
        public const string EventStatusInProgress = "IN_PROGRESS";
        public const string EventStatusCompleted = "COMPLETED";
        public const string EventStatusCancelled = "CANCELLED";

        public static readonly string[] Statuses = {
            EventStatusScheduled,
            EventStatusInProgress,
            EventStatusCompleted,
            EventStatusCancelled
        };

        public static int FromString(string status)
        {
            return status.ToUpper() switch
            {
                "SCHEDULED" => 0,
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
                0 => EventStatusScheduled,
                1 => EventStatusInProgress,
                2 => EventStatusCompleted,
                3 => EventStatusCancelled,
                _ => EventStatusScheduled
            };
        }

        public static bool IsValid(string status)
        {
            return Statuses.Contains(status.ToUpper());
        }
    }
}