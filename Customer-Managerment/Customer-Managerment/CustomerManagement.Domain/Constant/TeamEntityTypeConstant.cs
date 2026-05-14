namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class TeamEntityTypeConstant
    {
        public const string EntityTypeLead = "Lead";
        public const string EntityTypeDeal = "Deal";

        public static readonly string[] EntityTypes = {
            EntityTypeLead,
            EntityTypeDeal
        };

        public static bool IsValid(string entityType)
        {
            return EntityTypes.Contains(entityType);
        }
    }
}