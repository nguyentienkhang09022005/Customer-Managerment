using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Cases
{
    public class CaseHandler
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IMapper _mapper;

        public CaseHandler(ICaseRepository caseRepository, IMapper mapper)
        {
            _caseRepository = caseRepository;
            _mapper = mapper;
        }

        public async Task<List<CaseResponse>> GetListCasesAsync(Guid idUser)
        {
            var cases = await _caseRepository.GetListCasesAsync(idUser);
            return _mapper.Map<List<CaseResponse>>(cases);
        }

        public async Task<CaseResponse> GetCaseAsync(Guid idCase)
        {
            var caseEntity = await _caseRepository.GetCaseByIdAsync(idCase);
            return _mapper.Map<CaseResponse>(caseEntity);
        }

        public async Task<CaseResponse> CreateCaseAsync(CaseCreationRequest request)
        {
            var caseDomain = _mapper.Map<CaseDomain>(request);
            var newCase = await _caseRepository.AddCaseAsync(caseDomain);
            return _mapper.Map<CaseResponse>(newCase);
        }

        public async Task<CaseResponse> UpdateCaseAsync(CaseUpdateRequest request, Guid idCase)
        {
            var existing = await _caseRepository.GetExistingCaseAsync(idCase);
            if (existing == null)
                throw new DomainException("Không tìm thấy Case cần cập nhật!", 404);

            var caseDomain = _mapper.Map<CaseDomain>(existing);
            _mapper.Map(request, caseDomain);

            var updated = await _caseRepository.UpdateCaseAsync(caseDomain, existing);
            return _mapper.Map<CaseResponse>(updated);
        }

        public async Task<string> DeleteCaseAsync(Guid idCase)
        {
            await _caseRepository.DeleteCaseAsync(idCase);
            return "Xóa Case thành công!";
        }
    }
}
