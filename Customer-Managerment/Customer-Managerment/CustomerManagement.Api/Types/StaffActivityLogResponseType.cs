using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class StaffActivityLogResponseType : ObjectTypeExtension<StaffActivityLogResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<StaffActivityLogResponse> descriptor)
        {
            descriptor.Name("StaffActivityLogResponse");

            descriptor.Field(l => l.IdLog);
            descriptor.Field(l => l.IdStaff);
            descriptor.Field(l => l.StaffName);
            descriptor.Field(l => l.Action);
            descriptor.Field(l => l.EntityType);
            descriptor.Field(l => l.EntityId);
            descriptor.Field(l => l.Timestamp);
            descriptor.Field(l => l.IpAddress);
            descriptor.Field(l => l.UserAgent);
        }
    }
}