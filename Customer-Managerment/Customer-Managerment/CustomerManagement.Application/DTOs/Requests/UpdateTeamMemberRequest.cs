namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class UpdateTeamMemberRequest
    {
        public int? Role { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanDelete { get; set; }
    }
}