using HotChocolate.Types;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class UploadType : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Upload");
        }
    }
}