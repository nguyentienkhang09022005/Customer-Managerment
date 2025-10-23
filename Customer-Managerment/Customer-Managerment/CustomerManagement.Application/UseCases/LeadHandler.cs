using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Leads
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

        public async Task<List<LeadResponse>> GetAllLeadsAsync()
        {
            var leads = await _leadRepository.GetAllLeadsAsync();
            return _mapper.Map<List<LeadResponse>>(leads);
        }

        public async Task<LeadResponse> GetLeadByIdAsync(Guid idLead)
        {
            var lead = await _leadRepository.GetLeadByIdAsync(idLead);
            return _mapper.Map<LeadResponse>(lead);
        }

        public async Task<LeadResponse> CreateLeadAsync(LeadCreationRequest request)
        {
            var domain = _mapper.Map<LeadDomain>(request);
            var newLead = await _leadRepository.AddLeadAsync(domain);
            return _mapper.Map<LeadResponse>(newLead);
        }

        public async Task<LeadResponse> UpdateLeadAsync(Guid idLead, LeadUpdateRequest request)
        {
            var existingLead = await _leadRepository.GetExistingLeadAsync(idLead);
            if (existingLead == null)
                throw new DomainException("Không tìm thấy lead cần cập nhật!", 404);

            var domain = _mapper.Map<LeadDomain>(existingLead);
            _mapper.Map(request, domain);

            var updated = await _leadRepository.UpdateLeadAsync(domain, existingLead);
            return _mapper.Map<LeadResponse>(updated);
        }

        public async Task<string> DeleteLeadAsync(Guid idLead)
        {
            await _leadRepository.DeleteLeadAsync(idLead);
            return "Xóa lead thành công!";
        }
    }
}
