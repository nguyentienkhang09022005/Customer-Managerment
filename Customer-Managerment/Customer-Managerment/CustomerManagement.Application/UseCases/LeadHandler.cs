using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class LeadHandler
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;

        public LeadHandler(ILeadRepository leadRepository, IMapper mapper)
        {
            _leadRepository = leadRepository;
            _mapper = mapper;
        }

        public async Task<LeadResponse> CreateLeadAsync(LeadCreationRequest leadCreationRequest)
        {
            var checkEmail = await _leadRepository.checkPersonByEmailAsync(leadCreationRequest.Person.Email);
            if (checkEmail){
                throw new DomainException("Email đã tồn tại!", 409);
            }

            var leadDomain = _mapper.Map<LeadDomain>(leadCreationRequest);

            var createdLead = await _leadRepository.AddLeadAsync(leadDomain);

            var leadResponse = _mapper.Map<LeadResponse>(createdLead);
            leadResponse.personResponse = _mapper.Map<PersonResponse>(createdLead.personDomain);
            return leadResponse;
        }

        public async Task<string> DeleteLeadAsync(Guid idLead)
        {
            await _leadRepository.DeleteLeadAsync(idLead);

            return "Xóa khách hàng tiềm năng thành công!";
        }

        public async Task<LeadResponse> UpdateLeadAsync(LeadUpdateRequest leadUpdateRequest, Guid idLead)
        {
            var existLead = await _leadRepository.GetLeadByIdAsync(idLead);
            if (existLead == null){
                throw new DomainException("Khách hàng tiềm năng không tồn tại!", 404);
            }

            _mapper.Map(leadUpdateRequest, existLead);

            var updatedLead = await _leadRepository.UpdateLeadAsync(existLead);

            var leadResponse = _mapper.Map<LeadResponse>(updatedLead);
            leadResponse.personResponse = _mapper.Map<PersonResponse>(updatedLead.personDomain);
            return leadResponse;
        }

        public async Task<LeadResponse> GetLeadByIdAsync(Guid idLead)
        {
            var leadDomain = await _leadRepository.GetLeadByIdAsync(idLead);
            if (leadDomain == null)
            {
                throw new DomainException("Khách hàng tiềm năng không tồn tại!", 404);
            }
            return _mapper.Map<LeadResponse>(leadDomain);
        }

        public async Task<List<LeadResponse>> GetAllLeadsAsync()
        {
            var leadDomains = await _leadRepository.GetListLeadAsync();
            return _mapper.Map<List<LeadResponse>>(leadDomains);
        }
    }
}
