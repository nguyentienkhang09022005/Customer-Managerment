namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class PersonInput
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
    }

    public class PersonUpdateInput
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
    }
}