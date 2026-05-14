namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class AuditEntityTypeConstant
    {
        public const string EntityTypeStaff = "Staff";
        public const string EntityTypeLead = "Lead";
        public const string EntityTypeCustomer = "Customer";
        public const string EntityTypeContact = "Contact";
        public const string EntityTypeDeal = "Deal";
        public const string EntityTypeTask = "Task";
        public const string EntityTypeNote = "Note";
        public const string EntityTypeNotification = "Notification";
        public const string EntityTypeTeamMember = "TeamMember";

        public static readonly string[] EntityTypes = {
            EntityTypeStaff,
            EntityTypeLead,
            EntityTypeCustomer,
            EntityTypeContact,
            EntityTypeDeal,
            EntityTypeTask,
            EntityTypeNote,
            EntityTypeNotification,
            EntityTypeTeamMember
        };

        public static bool IsValid(string entityType)
        {
            return EntityTypes.Contains(entityType);
        }
    }
}