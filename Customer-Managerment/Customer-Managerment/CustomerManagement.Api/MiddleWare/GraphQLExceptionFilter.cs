using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Api.MiddleWare
{
    public class GraphQLExceptionFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is DomainException domainEx)
            {
                return ErrorBuilder.New()
                    .SetMessage(domainEx.Message)
                    .SetExtension("status", domainEx.StatusCode)
                    .Build();
            }

            // fallback cho lỗi khác
            return ErrorBuilder.New()
                .SetMessage("Lỗi hệ thống không xác định.")
                .SetExtension("status", 500)
                .Build();
        }
    }
}
