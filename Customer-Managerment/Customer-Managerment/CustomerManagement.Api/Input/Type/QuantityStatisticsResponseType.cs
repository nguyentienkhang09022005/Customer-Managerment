using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class QuantityStatisticsResponseType : ObjectType<QuantityStatisticsResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<QuantityStatisticsResponse> descriptor)
        {
            descriptor
                .Field(t => t.quantityStatisticsDetailContactResponse)
                .ResolveWith<StatisticsResolvers>(r => r.GetContactDetails(default!, default!));

            descriptor
                .Field(t => t.quantityStatisticsDetailDealResponse)
                .ResolveWith<StatisticsResolvers>(r => r.GetDealDetails(default!, default!));
        }
    }

    public class StatisticsResolvers
    {
        public async Task<QuantityStatisticsDetailContactResponse> GetContactDetails(
            [Parent] QuantityStatisticsResponse response,
            [Service] IContactRepository contactRepository)
        {
            return await contactRepository.QuantityStatisticsDetailContactResponse();
        }

        public async Task<QuantityStatisticsDetailDealResponse> GetDealDetails(
            [Parent] QuantityStatisticsResponse response,
            [Service] IDealRepository dealRepository)
        {
            return await dealRepository.QuantityStatisticsDetailDealResponse();
        }
    }
}
