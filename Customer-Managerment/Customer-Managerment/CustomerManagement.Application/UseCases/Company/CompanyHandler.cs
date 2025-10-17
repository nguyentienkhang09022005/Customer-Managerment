using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Company
{
    public class CompanyHandler
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<List<CompanyResponse>> GetListCompanyAsync()
        {
            var companies = await _companyRepository.GetListCompanyAsync();
            return _mapper.Map<List<CompanyResponse>>(companies);
        }

        public async Task<CompanyResponse> GetInfCompanyAsync(Guid idCompany)
        {
            var infCompany = await _companyRepository.GetCompanyByIdAsync(idCompany);
            return _mapper.Map<CompanyResponse>(infCompany);
        }

        public async Task<CompanyResponse> CreateCompanyAsync(CompanyRequest companyRequest)
        {
            var companyDomain = _mapper.Map<CompanyDomain>(companyRequest);
            var newCompany = await _companyRepository.AddCompanyAsync(companyDomain);

            var companyResponse = _mapper.Map<CompanyResponse>(newCompany);
            return companyResponse;
        }

        public async Task<CompanyResponse> UpdateCompanyAsync(CompanyRequest companyRequest, Guid idCompany)
        {
            var companyDomain = _mapper.Map<CompanyDomain>(companyRequest);
            var updatedCompany = await _companyRepository.UpdateCompanyAsync(companyDomain, idCompany);

            return _mapper.Map<CompanyResponse>(updatedCompany);
        }

        public async Task<string> DeleteCompanyAsync(Guid idCompany)
        {
            await _companyRepository.DeleteCompanyAsync(idCompany);
            return "Xóa công ty thành công!";
        }
    }
}
