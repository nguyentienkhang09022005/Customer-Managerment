using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class OpportunityHandler
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IMapper _mapper;

        public OpportunityHandler(IOpportunityRepository opportunityRepository, IMapper mapper)
        {
            _opportunityRepository = opportunityRepository;
            _mapper = mapper;
        }

        public async Task<List<OpportunityResponse>> GetListOpportunitiesAsync(Guid idUser)
        {
            var list = await _opportunityRepository.GetListOpportunitiesAsync(idUser);
            return _mapper.Map<List<OpportunityResponse>>(list);
        }

        public async Task<OpportunityResponse> GetInfOpportunityAsync(Guid idOpportunity)
        {
            var opp = await _opportunityRepository.GetOpportunityByIdAsync(idOpportunity);
            return _mapper.Map<OpportunityResponse>(opp);
        }

        public async Task<OpportunityResponse> CreateOpportunityAsync(OpportunityCreationRequest request)
        {
            var domain = _mapper.Map<OpportunityDomain>(request);
            var newOpp = await _opportunityRepository.AddOpportunityAsync(domain);
            return _mapper.Map<OpportunityResponse>(newOpp);
        }

        public async Task<OpportunityResponse> UpdateOpportunityAsync(OpportunityUpdateRequest opportunityUpdateRequest, Guid idOpportunity)
        {
            var opportunityEntity = await _opportunityRepository.GetExistingOpportunityAsync(idOpportunity);
            if (opportunityEntity == null)
            {
                throw new DomainException("Không tìm thấy cơ hội cần đổi thông tin", 404);
            }
            var opportunityDomain = _mapper.Map<OpportunityDomain>(opportunityEntity);

            _mapper.Map(opportunityUpdateRequest, opportunityDomain);

            var updatedOpportunity = await _opportunityRepository.UpdateOpportunityAsync(opportunityDomain, opportunityEntity);

            return _mapper.Map<OpportunityResponse>(updatedOpportunity);
        }

        public async Task<string> DeleteOpportunityAsync(Guid idOpportunity)
        {
            await _opportunityRepository.DeleteOpportunityAsync(idOpportunity);
            return "Xóa cơ hội thành công!";
        }
    }
}
