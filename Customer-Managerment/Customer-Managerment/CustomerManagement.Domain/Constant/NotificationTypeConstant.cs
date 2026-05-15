namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class NotificationTypeConstant
    {
        public const string NotificationTaskAssigned = "TASK_ASSIGNED";
        public const string NotificationTaskCompleted = "TASK_COMPLETED";
        public const string NotificationDealUpdated = "DEAL_UPDATED";
        public const string NotificationContactStatusChanged = "CONTACT_STATUS_CHANGED";
        public const string NotificationMention = "MENTION";
        public const string NotificationSystem = "SYSTEM";
        public const string NotificationReminder = "REMINDER";

        public static readonly string[] NotificationTypes = {
            NotificationTaskAssigned,
            NotificationTaskCompleted,
            NotificationDealUpdated,
            NotificationContactStatusChanged,
            NotificationMention,
            NotificationSystem,
            NotificationReminder
        };
    }
}