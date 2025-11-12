using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CustomerQuery
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerQuery(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<CustomerResponse> GetCustomers()
        {
            var customers = _customerRepository.GetListCustomer();
            return customers.ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider);
        }

        public IQueryable<CustomerResponse> GetCustomerById(Guid idCustomer)
        {
            var customer = _customerRepository.GetCustomerById(idCustomer);
            return customer.ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider);
        }
    }
}
