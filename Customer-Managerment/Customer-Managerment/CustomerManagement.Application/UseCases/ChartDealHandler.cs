using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ChartDealHandler
    {
        private readonly IDealRepository _dealRepository;
        private readonly IMapper _mapper;

        public ChartDealHandler(IDealRepository dealRepository, IMapper mapper)
        {
            _dealRepository = dealRepository;
            _mapper = mapper;
        }

        public async Task<ChartDealResponse> ChartDealResponseAsync()
        {
            var dealDomains = await _dealRepository.GetListDeal().ToListAsync();

            var wonDeals = dealDomains.Where(d => d.Status == StatuDealConstant.DealWon).ToList();
            decimal totalWonAmount = wonDeals.Sum(d => d.Price);

            var lostDeals = dealDomains.Where(d => d.Status == StatuDealConstant.DealLost).ToList();
            decimal totalLostAmount = lostDeals.Sum(d => d.Price);

            return new ChartDealResponse
            {
                SuccessfullDealValue = totalWonAmount,
                FailedDealValue = totalLostAmount,
                ListSuccessfullDeal = _mapper.Map<List<ListSuccessfullDealResponse>>(wonDeals),
                ListFailedDeal = _mapper.Map<List<ListFailedDealResponse>>(lostDeals)
            };

        }
    }
}
