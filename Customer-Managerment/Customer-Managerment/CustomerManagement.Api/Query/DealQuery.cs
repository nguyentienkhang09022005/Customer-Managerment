using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class DealQuery
    {
        private readonly IDealRepository _dealRepository;
        private readonly IMapper _mapper;

        public DealQuery(IDealRepository dealRepository, IMapper mapper) 
        {
            _dealRepository = dealRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<DealResponse> GetDeals()
        {
            var deals = _dealRepository.GetListDeal();
            return deals.ProjectTo<DealResponse>(_mapper.ConfigurationProvider);
        }

        public IQueryable<DealResponse> GetContactById(Guid idContact)
        {
            var deal = _dealRepository.GetDealById(idContact);
            return deal.ProjectTo<DealResponse>(_mapper.ConfigurationProvider);
        }
    }
}
