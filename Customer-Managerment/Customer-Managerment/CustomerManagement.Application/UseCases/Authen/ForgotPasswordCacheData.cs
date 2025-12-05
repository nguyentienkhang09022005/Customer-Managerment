namespace Customer_Managerment.CustomerManagement.Application.UseCases.Authen
{
    public class ForgotPasswordCacheData
    {
        public required string Otp { get; set; }

        public required string Email { get; set; }
    }
}
