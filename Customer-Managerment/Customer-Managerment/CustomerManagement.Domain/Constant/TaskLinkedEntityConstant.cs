namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class TaskLinkedEntityConstant
    {
        public const string LinkedEntityLead = "Lead";
        public const string LinkedEntityCustomer = "Customer";
        public const string LinkedEntityDeal = "Deal";

        public static readonly string[] LinkedEntities = {
            LinkedEntityLead,
            LinkedEntityCustomer,
            LinkedEntityDeal
        };

        public static bool IsValid(string? entityType)
        {
            if (string.IsNullOrEmpty(entityType)) return true;
            return LinkedEntities.Contains(entityType);
        }
    }
}