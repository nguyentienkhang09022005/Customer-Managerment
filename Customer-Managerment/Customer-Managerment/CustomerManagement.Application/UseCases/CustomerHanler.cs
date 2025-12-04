using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

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
            await _elasticsearchService.IndexAsync(customerResponse, "customers");
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
    }
}
