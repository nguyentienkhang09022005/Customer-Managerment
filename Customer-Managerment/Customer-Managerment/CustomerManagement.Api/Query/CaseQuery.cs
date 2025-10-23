using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Cases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CaseQuery
    {
        private readonly CaseHandler _handler;

        public CaseQuery(CaseHandler handler)
        {
            _handler = handler;
        }

        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<CaseResponse>> GetCasesAsync(Guid idUser)
        {
            return await _handler.GetListCasesAsync(idUser);
        }

        [Authorize]
        public async Task<CaseResponse> GetCaseAsync(Guid idCase)
        {
            return await _handler.GetCaseAsync(idCase);
        }
    }
}
