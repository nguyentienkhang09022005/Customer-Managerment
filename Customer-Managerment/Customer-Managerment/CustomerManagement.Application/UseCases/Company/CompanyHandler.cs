using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

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

        public async Task<List<CompanyDTO>> GetListCompanyAsync()
        {
            var companies = await _companyRepository.GetListCompanyAsync();
            return _mapper.Map<List<CompanyDTO>>(companies);
        }
    }
}
