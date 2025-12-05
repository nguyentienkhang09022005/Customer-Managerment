using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using OfficeOpenXml;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class CustomerHanler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;

        public CustomerHanler(ICustomerRepository customerRepository, 
                              ILeadRepository leadRepository, 
                              IMapper mapper, 
                              IElasticsearchService elasticsearchService)
        {
            _customerRepository = customerRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerCreationRequest customerCreationRequest)
        {
            var checkEmail = await _leadRepository.checkPersonByEmailAsync(customerCreationRequest.Person.Email);
            if (checkEmail)
            {
                throw new DomainException("Email đã tồn tại!", 409);
            }

            var customerDomain = _mapper.Map<CustomerDomain>(customerCreationRequest);

            var createdCustomer = await _customerRepository.AddCustomerAsync(customerDomain);

            var customerResponse = _mapper.Map<CustomerResponse>(createdCustomer);
            customerResponse.personResponse = _mapper.Map<PersonResponse>(createdCustomer.personDomain);
            //await _elasticsearchService.IndexAsync(customerResponse, "customers");
            return customerResponse;
        }

        public async Task<string> DeletecCustomerAsync(Guid idCustomer)
        {
            await _customerRepository.DeleteCustomerAsync(idCustomer);
            await _elasticsearchService.DeleteAsync<CustomerResponse>(idCustomer.ToString(), "customers"); // Xóa khỏi Elasticsearch

            return "Xóa khách hàng thành công!";
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(CustomerUpdateRequest customerUpdateRequest, Guid idCustomer)
        {
            var existCustomer = await _customerRepository.GetCustomerByIdAsync(idCustomer);
            if (existCustomer == null){
                throw new DomainException("Khách hàng không tồn tại!", 404);
            }

            _mapper.Map(customerUpdateRequest, existCustomer);

            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(existCustomer);

            var customerResponse = _mapper.Map<CustomerResponse>(updatedCustomer);
            customerResponse.personResponse = _mapper.Map<PersonResponse>(updatedCustomer.personDomain);
            return customerResponse;
        }
        public async Task<string> ImportCustomerExcelAsync(IFile file)
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
                var customerRequest = new CustomerCreationRequest
                {
                    Person = new PersonCreationRequest
                    {
                        Fullname = worksheet.Cells[row, 1].Text,
                        Email = worksheet.Cells[row, 2].Text,
                        Phone = worksheet.Cells[row, 3].Text,
                        Salary = decimal.TryParse(worksheet.Cells[row, 4].Text, out var salary) ? salary : 0,
                        Location = worksheet.Cells[row, 5].Text
                    }
                };

                try
                {
                    await CreateCustomerAsync(customerRequest);
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
