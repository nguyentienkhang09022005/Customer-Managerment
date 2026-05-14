namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class TeamRoleConstant
    {
        public const string RoleOwner = "OWNER";
        public const string RoleMember = "MEMBER";
        public const string RoleViewer = "VIEWER";

        public static readonly string[] Roles = {
            RoleOwner,
            RoleMember,
            RoleViewer
        };

        public static int FromString(string role)
        {
            return role.ToUpper() switch
            {
                "OWNER" => 0,
                "MEMBER" => 1,
                "VIEWER" => 2,
                _ => 1
            };
        }

        public static string ToString(int role)
        {
            return role switch
            {
                0 => RoleOwner,
                1 => RoleMember,
                2 => RoleViewer,
                _ => RoleMember
            };
        }

        public static bool IsValid(string role)
        {
            return Roles.Contains(role.ToUpper());
        }
    }
}