using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class StaffStatusResponseType : ObjectTypeExtension<StaffStatusResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<StaffStatusResponse> descriptor)
        {
            descriptor.Name("StaffStatusResponse");

            descriptor.Field(s => s.IdStaff);
            descriptor.Field(s => s.Fullname);
            descriptor.Field(s => s.Email);
            descriptor.Field(s => s.Status);
            descriptor.Field(s => s.StatusName);
            descriptor.Field(s => s.LastActiveAt);
        }
    }
}