using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
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
    }
}
