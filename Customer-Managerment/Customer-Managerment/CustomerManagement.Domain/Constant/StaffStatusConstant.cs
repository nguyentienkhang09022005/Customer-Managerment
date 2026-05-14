namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class StaffStatusConstant
    {
        public const string StaffStatusOffline = "OFFLINE";
        public const string StaffStatusOnline = "ONLINE";
        public const string StaffStatusBusy = "BUSY";
        public const string StaffStatusAway = "AWAY";

        public static readonly string[] Statuses = {
            StaffStatusOffline,
            StaffStatusOnline,
            StaffStatusBusy,
            StaffStatusAway
        };

        public static int FromString(string status)
        {
            return status.ToUpper() switch
            {
                "OFFLINE" => 0,
                "ONLINE" => 1,
                "BUSY" => 2,
                "AWAY" => 3,
                _ => 0
            };
        }

        public static string ToString(int status)
        {
            return status switch
            {
                0 => StaffStatusOffline,
                1 => StaffStatusOnline,
                2 => StaffStatusBusy,
                3 => StaffStatusAway,
                _ => StaffStatusOffline
            };
        }

        public static bool IsValid(string status)
        {
            return Statuses.Contains(status.ToUpper());
        }
    }
}