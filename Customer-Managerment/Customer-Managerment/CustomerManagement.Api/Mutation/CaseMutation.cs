using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Cases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CaseMutation
    {
        private readonly CaseHandler _handler;

        public CaseMutation(CaseHandler handler)
        {
            _handler = handler;
        }

        //[Authorize]
        public async Task<CaseResponse> CreateCaseAsync(CaseCreationRequest request)
        {
            return await _handler.CreateCaseAsync(request);
        }

        //[Authorize]
        public async Task<CaseResponse> UpdateCaseAsync(CaseUpdateRequest request, Guid idCase)
        {
            return await _handler.UpdateCaseAsync(request, idCase);
        }

        //[Authorize]
        public async Task<string> DeleteCaseAsync(Guid idCase)
        {
            return await _handler.DeleteCaseAsync(idCase);
        }
    }
}
