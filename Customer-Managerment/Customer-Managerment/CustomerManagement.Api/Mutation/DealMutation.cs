using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class DealMutation
    {
        private readonly DealHandler _dealHandler;

        public DealMutation(DealHandler dealHandler)
        {
            _dealHandler = dealHandler;
        }

        public async Task<DealResponse> CreateDealAsync(DealCreationRequest dealCreationRequest)
        {
            return await _dealHandler.CreateDealAsync(dealCreationRequest);
        }

        public async Task<string> DeleteDealAsync(Guid idDeal)
        {
            return await _dealHandler.DeleteDealAsync(idDeal);
        }

        public async Task<DealResponse> UpdateDealAsync(DealUpdateRequest dealUpdateRequest, Guid idDeal)
        {
            return await _dealHandler.UpdateDealAsync(dealUpdateRequest, idDeal);
        }
    }
}
