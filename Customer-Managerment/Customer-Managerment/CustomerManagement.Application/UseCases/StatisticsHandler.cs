using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class StatisticsHandler
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IDealRepository _dealRepository;

        public StatisticsHandler(ILeadRepository leadRepository, 
                                 ICustomerRepository customerRepository,
                                 IContactRepository contactRepository,
                                 IDealRepository dealRepository)
        {
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
            _contactRepository = contactRepository;
            _dealRepository = dealRepository;
        }

        public async Task<QuantityStatisticsResponse> GetQuantityStatisticsResponseAsync()
        {
            var totalProfit = await _dealRepository.GetTotalProfitAsync();
            var totalLeads = await _leadRepository.getTotalLeadsAsync();
            var totalCustomers = await _customerRepository.getTotalCustomersAsync();
            var totalContacts = await _contactRepository.getTotalContactsAsync();
            var totalDeals = await _dealRepository.getTotalDealsAsync();
            var statisticsResponse = new QuantityStatisticsResponse
            {
                TotalProfit = totalProfit,
                QuantityLeads = totalLeads,
                QuantityCustomers = totalCustomers,
                QuantityContacts = totalContacts,
                QuantityDeals = totalDeals
            };
            return statisticsResponse;
        }
    }
}
