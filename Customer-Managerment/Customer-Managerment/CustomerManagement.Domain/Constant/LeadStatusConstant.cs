namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public static class LeadStatusConstant
    {
        public const int LeadStatusNew = 0;
        public const int LeadStatusContacted = 1;
        public const int LeadStatusQualified = 2;
        public const int LeadStatusConverted = 3;
        public const int LeadStatusLost = 4;

        public static string ToString(int status)
        {
            return status switch
            {
                LeadStatusNew => "NEW",
                LeadStatusContacted => "CONTACTED",
                LeadStatusQualified => "QUALIFIED",
                LeadStatusConverted => "CONVERTED",
                LeadStatusLost => "LOST",
                _ => "UNKNOWN"
            };
        }
    }
}