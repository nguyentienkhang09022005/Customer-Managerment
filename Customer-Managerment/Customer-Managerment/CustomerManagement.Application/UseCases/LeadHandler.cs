using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using OfficeOpenXml;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class LeadHandler
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IMapper _mapper;

        public LeadHandler(ILeadRepository leadRepository, 
                           IMapper mapper, 
                           IElasticsearchService elasticsearchService)
        {
            _leadRepository = leadRepository;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
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
            //await _elasticsearchService.IndexAsync(leadResponse, "leads");

            return leadResponse;
        }

        public async Task<string> DeleteLeadAsync(Guid idLead)
        {
            await _leadRepository.DeleteLeadAsync(idLead);
            await _elasticsearchService.DeleteAsync<LeadResponse>(idLead.ToString(), "leads"); // Xóa khỏi Elasticsearch

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

        public async Task<string> ImportLeadExcelAsync(IFile file)
        {
            if (file == null || file.Length == 0)
                throw new DomainException("File không hợp lệ!", 400);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            var importedCount = 0;

            for (int row = 2; row <= rowCount; row++)
            {
                var leadRequest = new LeadCreationRequest
                {
                    Resource = worksheet.Cells[row, 1].Text,
                    Person = new PersonCreationRequest
                    {
                        Fullname = worksheet.Cells[row, 2].Text,
                        Email = worksheet.Cells[row, 3].Text,
                        Phone = worksheet.Cells[row, 4].Text,
                        Salary = decimal.TryParse(worksheet.Cells[row, 5].Text, out var salary) ? salary : 0,
                        Location = worksheet.Cells[row, 6].Text
                    }
                };

                try
                {
                    await CreateLeadAsync(leadRequest);
                    importedCount++;
                }
                catch (DomainException ex)
                {
                    if (ex.StatusCode == 409) continue;
                    throw;
                }
            }

            return "Tải lên file excel thành công!";
        }
    }
}
