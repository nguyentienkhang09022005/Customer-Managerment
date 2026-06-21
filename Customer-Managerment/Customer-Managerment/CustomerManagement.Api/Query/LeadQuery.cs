using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class LeadQuery
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;

        public LeadQuery(ILeadRepository leadRepository, IMapper mapper)
        {
            _leadRepository = leadRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<LeadResponse> GetLeads()
        {
            var leads = _leadRepository.GetListLead();
            return leads.ProjectTo<LeadResponse>(_mapper.ConfigurationProvider);
        }

        public IQueryable<LeadResponse> GetLeadById(Guid idLead)
        {
            var lead = _leadRepository.GetLeadById(idLead);
            return lead.ProjectTo<LeadResponse>(_mapper.ConfigurationProvider);
        }

        public async Task<PagedResponse<LeadResponse>> GetLeadsPaged(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200;

            var (items, totalCount) = await _leadRepository.GetListLeadPagedAsync(page, pageSize);
            var dtoItems = _mapper.Map<List<LeadResponse>>(items);
            return new PagedResponse<LeadResponse> { Items = dtoItems, TotalCount = totalCount };
        }
    }
}
