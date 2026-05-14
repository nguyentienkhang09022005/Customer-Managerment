namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class AuditActionConstant
    {
        public const string ActionCreate = "CREATE";
        public const string ActionUpdate = "UPDATE";
        public const string ActionDelete = "DELETE";
        public const string ActionRestore = "RESTORE";
        public const string ActionAssign = "ASSIGN";
        public const string ActionLogin = "LOGIN";
        public const string ActionLogout = "LOGOUT";

        public static readonly string[] Actions = {
            ActionCreate,
            ActionUpdate,
            ActionDelete,
            ActionRestore,
            ActionAssign,
            ActionLogin,
            ActionLogout
        };

        public static bool IsValid(string action)
        {
            return Actions.Contains(action.ToUpper());
        }
    }
}