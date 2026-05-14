using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class TeamMemberResponseType : ObjectTypeExtension<TeamMemberResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<TeamMemberResponse> descriptor)
        {
            descriptor.Name("TeamMemberResponse");

            descriptor.Field(t => t.Id);
            descriptor.Field(t => t.EntityType);
            descriptor.Field(t => t.EntityId);
            descriptor.Field(t => t.IdStaff);
            descriptor.Field(t => t.Staff);
            descriptor.Field(t => t.Role);
            descriptor.Field(t => t.AssignedAt);
            descriptor.Field(t => t.AssignedBy);
            descriptor.Field(t => t.CanEdit);
            descriptor.Field(t => t.CanDelete);
        }
    }
}