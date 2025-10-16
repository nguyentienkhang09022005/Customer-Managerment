namespace Customer_Managerment.CustomerManagement.Application.UseCases.Authen
{
    public class RegisterCacheData
    {
        public required string Otp { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
