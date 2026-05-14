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
    public class CustomerHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;

        public CustomerHandler(ICustomerRepository customerRepository,
                              ILeadRepository leadRepository,
                              IMapper mapper,
                              IElasticsearchService elasticsearchService)
        {
            _customerRepository = customerRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerCreationRequest request)
        {
            ValidateCustomerCreation(request);

            var checkEmail = await _leadRepository.CheckPersonByEmailAsync(request.Email);
            if (checkEmail)
            {
                throw new EmailAlreadyExistsException();
            }

            var customer = _mapper.Map<Person>(request);
            customer.Discriminator = PersonType.Customer;

            var createdCustomer = await _customerRepository.AddCustomerAsync(customer);
            var response = _mapper.Map<CustomerResponse>(createdCustomer);

            await _elasticsearchService.IndexAsync(response, "customers");

            return response;
        }

        public async Task<string> DeleteCustomerAsync(Guid idCustomer)
        {
            var result = await _customerRepository.SoftDeleteCustomerAsync(idCustomer);
            if (!result)
            {
                throw new CustomerNotFoundException();
            }

            await _elasticsearchService.DeleteAsync<CustomerResponse>(idCustomer.ToString(), "customers");
            return "Xóa khách hàng thành công!";
        }

        public async Task<string> ImportCustomerExcelAsync(IFile file)
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

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullname))
                    continue;

                if (!IsValidEmail(email))
                    continue;

                var existingEmail = await _leadRepository.CheckPersonByEmailAsync(email);
                if (existingEmail)
                    continue;

                var customer = new Person
                {
                    Email = email,
                    Fullname = fullname,
                    Phone = phone,
                    Location = location,
                    Discriminator = PersonType.Customer
                };

                await _customerRepository.AddCustomerAsync(customer);
                importedCount++;
            }

            return $"Đã import thành công {importedCount} customers!";
        }

        public async Task<CustomerResponse> RestoreCustomerAsync(Guid idCustomer)
        {
            var result = await _customerRepository.RestoreCustomerAsync(idCustomer);
            if (!result)
            {
                throw new CustomerNotFoundException();
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(idCustomer);
            var response = _mapper.Map<CustomerResponse>(customer);

            await _elasticsearchService.IndexAsync(response, "customers");
            return response;
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(CustomerUpdateRequest request, Guid idCustomer)
        {
            ValidateCustomerUpdate(request);

            var existingCustomer = await _customerRepository.GetCustomerByIdAsync(idCustomer);
            if (existingCustomer == null)
            {
                throw new CustomerNotFoundException();
            }

            var checkEmail = await _leadRepository.CheckPersonByEmailAsync(request.Email);
            if (checkEmail && existingCustomer.Email != request.Email)
            {
                throw new EmailAlreadyExistsException();
            }

            existingCustomer.Fullname = request.Fullname;
            existingCustomer.Email = request.Email;
            existingCustomer.Phone = request.Phone;
            existingCustomer.Location = request.Location;
            existingCustomer.UpdatedAt = DateTime.UtcNow;

            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(existingCustomer);
            var response = _mapper.Map<CustomerResponse>(updatedCustomer);

            await _elasticsearchService.IndexAsync(response, "customers");
            return response;
        }

        public async Task<CustomerResponse> GetCustomerByIdAsync(Guid idCustomer)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(idCustomer);
            return _mapper.Map<CustomerResponse>(customer);
        }

        private void ValidateCustomerCreation(CustomerCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Fullname))
                throw new RequiredFieldException("Fullname");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RequiredFieldException("Email");

            if (!IsValidEmail(request.Email))
                throw new InvalidEmailException();
        }

        private void ValidateCustomerUpdate(CustomerUpdateRequest request)
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