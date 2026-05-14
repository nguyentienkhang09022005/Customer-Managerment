using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using OfficeOpenXml;
using System.Text.RegularExpressions;

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

        public async Task<LeadResponse> CreateLeadAsync(LeadCreationRequest request)
        {
            ValidateLeadCreation(request);

            var checkEmail = await _leadRepository.CheckPersonByEmailAsync(request.Email);
            if (checkEmail)
            {
                throw new EmailAlreadyExistsException();
            }

            var lead = _mapper.Map<Person>(request);
            lead.Discriminator = PersonType.Lead;

            var createdLead = await _leadRepository.AddLeadAsync(lead);
            var response = _mapper.Map<LeadResponse>(createdLead);

            await _elasticsearchService.IndexAsync(response, "leads");

            return response;
        }

        public async Task<string> DeleteLeadAsync(Guid idLead)
        {
            var result = await _leadRepository.SoftDeleteLeadAsync(idLead);
            if (!result)
            {
                throw new LeadNotFoundException();
            }

            await _elasticsearchService.DeleteAsync<LeadResponse>(idLead.ToString(), "leads");
            return "Xóa khách hàng tiềm năng thành công!";
        }

        public async Task<string> ImportLeadExcelAsync(IFile file)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("File không được để trống!");

            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            if (rowCount < 2)
                throw new ValidationException("File Excel không có dữ liệu!");

            var importedCount = 0;

            for (int row = 2; row <= rowCount; row++)
            {
                var email = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                var fullname = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                var phone = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                var location = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                var resource = worksheet.Cells[row, 5].Value?.ToString()?.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullname))
                    continue;

                if (!IsValidEmail(email))
                    continue;

                var existingEmail = await _leadRepository.CheckPersonByEmailAsync(email);
                if (existingEmail)
                    continue;

                var lead = new Person
                {
                    Email = email,
                    Fullname = fullname,
                    Phone = phone,
                    Location = location,
                    Resource = resource,
                    Discriminator = PersonType.Lead
                };

                await _leadRepository.AddLeadAsync(lead);
                importedCount++;
            }

            return $"Đã import thành công {importedCount} leads!";
        }

        public async Task<LeadResponse> RestoreLeadAsync(Guid idLead)
        {
            var result = await _leadRepository.RestoreLeadAsync(idLead);
            if (!result)
            {
                throw new LeadNotFoundException();
            }

            var lead = await _leadRepository.GetLeadByIdAsync(idLead);
            var response = _mapper.Map<LeadResponse>(lead);

            await _elasticsearchService.IndexAsync(response, "leads");
            return response;
        }

        public async Task<LeadResponse> UpdateLeadAsync(LeadUpdateRequest request, Guid idLead)
        {
            ValidateLeadUpdate(request);

            var existingLead = await _leadRepository.GetLeadByIdAsync(idLead);
            if (existingLead == null)
            {
                throw new LeadNotFoundException();
            }

            var checkEmail = await _leadRepository.CheckPersonByEmailAsync(request.Email);
            if (checkEmail && existingLead.Email != request.Email)
            {
                throw new EmailAlreadyExistsException();
            }

            existingLead.Fullname = request.Fullname;
            existingLead.Email = request.Email;
            existingLead.Phone = request.Phone;
            existingLead.Location = request.Location;
            existingLead.Resource = request.Resource;
            existingLead.UpdatedAt = DateTime.UtcNow;

            var updatedLead = await _leadRepository.UpdateLeadAsync(existingLead);
            var response = _mapper.Map<LeadResponse>(updatedLead);

            await _elasticsearchService.IndexAsync(response, "leads");
            return response;
        }

        public async Task<LeadResponse> GetLeadByIdAsync(Guid idLead)
        {
            var lead = await _leadRepository.GetLeadByIdAsync(idLead);
            return _mapper.Map<LeadResponse>(lead);
        }

        private void ValidateLeadCreation(LeadCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Fullname))
                throw new RequiredFieldException("Fullname");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RequiredFieldException("Email");

            if (!IsValidEmail(request.Email))
                throw new InvalidEmailException();
        }

        private void ValidateLeadUpdate(LeadUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Fullname))
                throw new RequiredFieldException("Fullname");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RequiredFieldException("Email");

            if (!IsValidEmail(request.Email))
                throw new InvalidEmailException();
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}