using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class AuditLogResponseType : ObjectTypeExtension<AuditLogResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<AuditLogResponse> descriptor)
        {
            descriptor.Name("AuditLogResponse");

            descriptor.Field(a => a.IdLog);
            descriptor.Field(a => a.Action);
            descriptor.Field(a => a.EntityType);
            descriptor.Field(a => a.EntityId);
            descriptor.Field(a => a.OldValues);
            descriptor.Field(a => a.NewValues);
            descriptor.Field(a => a.IdStaff);
            descriptor.Field(a => a.StaffName);
            descriptor.Field(a => a.IpAddress);
            descriptor.Field(a => a.UserAgent);
            descriptor.Field(a => a.Timestamp);
            descriptor.Field(a => a.Description);
        }
    }
}