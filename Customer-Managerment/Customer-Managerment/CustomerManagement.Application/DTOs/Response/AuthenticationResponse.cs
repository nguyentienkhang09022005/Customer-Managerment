namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class AuthenticationResponse
    {
        public string? Token { get; set; }

        public StaffResponse? InfStaff { get; set; }
    }
}
