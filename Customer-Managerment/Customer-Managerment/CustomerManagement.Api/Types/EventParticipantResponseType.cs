using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class EventParticipantResponseType : ObjectType<EventParticipantResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<EventParticipantResponse> descriptor)
        {
            descriptor.Name("EventParticipantResponse");

            descriptor.Field(p => p.Id);
            descriptor.Field(p => p.IdEvent);
            descriptor.Field(p => p.IdStaff);
            descriptor.Field(p => p.Status);
            descriptor.Field(p => p.RespondedAt);
        }
    }
}