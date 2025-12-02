using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class StatisticsQuery
    {
        private readonly StatisticsHandler _statisticsHandler;
        private readonly ChartDealHandler _chartDealHandler;

        public StatisticsQuery(StatisticsHandler statisticsHandler, ChartDealHandler chartDealHandler)
        {
            _statisticsHandler = statisticsHandler;
            _chartDealHandler = chartDealHandler;
        }

        public async Task<QuantityStatisticsResponse> GetStatistics()
        {
            return await _statisticsHandler.GetQuantityStatisticsResponseAsync();
        }

        public async Task<ChartDealResponse> GetChartDeal()
        {
            return await _chartDealHandler.ChartDealResponseAsync();
        }
    }
}
