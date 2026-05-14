namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ConfirmOTPRequest
    {
        public required string Email { get; set; }

        public required string OTP { get; set; }
    }
}
