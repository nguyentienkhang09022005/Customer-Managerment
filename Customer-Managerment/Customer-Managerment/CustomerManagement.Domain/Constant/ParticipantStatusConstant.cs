namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class ParticipantStatusConstant
    {
        public const string StatusPending = "PENDING";
        public const string StatusAccepted = "ACCEPTED";
        public const string StatusDeclined = "DECLINED";
        public const string StatusTentative = "TENTATIVE";

        public static readonly string[] Statuses = {
            StatusPending,
            StatusAccepted,
            StatusDeclined,
            StatusTentative
        };

        public static int FromString(string status)
        {
            return status.ToUpper() switch
            {
                "PENDING" => 0,
                "ACCEPTED" => 1,
                "DECLINED" => 2,
                "TENTATIVE" => 3,
                _ => 0
            };
        }

        public static string ToString(int status)
        {
            return status switch
            {
                0 => StatusPending,
                1 => StatusAccepted,
                2 => StatusDeclined,
                3 => StatusTentative,
                _ => StatusPending
            };
        }

        public static bool IsValid(string status)
        {
            return Statuses.Contains(status.ToUpper());
        }
    }
}