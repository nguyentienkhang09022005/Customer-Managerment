namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class CalendarEventTypeConstant
    {
        public const string EventTypeMeeting = "MEETING";
        public const string EventTypeCall = "CALL";
        public const string EventTypeTaskDeadline = "TASK_DEADLINE";
        public const string EventTypeFollowUp = "FOLLOW_UP";

        public static readonly string[] EventTypes = {
            EventTypeMeeting,
            EventTypeCall,
            EventTypeTaskDeadline,
            EventTypeFollowUp
        };

        public static int FromString(string eventType)
        {
            return eventType.ToUpper() switch
            {
                "MEETING" => 0,
                "CALL" => 1,
                "TASK_DEADLINE" => 2,
                "FOLLOW_UP" => 3,
                _ => 0
            };
        }

        public static string ToString(int eventType)
        {
            return eventType switch
            {
                0 => EventTypeMeeting,
                1 => EventTypeCall,
                2 => EventTypeTaskDeadline,
                3 => EventTypeFollowUp,
                _ => EventTypeMeeting
            };
        }

        public static bool IsValid(string eventType)
        {
            return EventTypes.Contains(eventType.ToUpper());
        }
    }
}