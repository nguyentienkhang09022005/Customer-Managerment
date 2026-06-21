using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
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

        public async Task<PagedResponse<CustomerResponse>> GetCustomersPaged(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200;

            var (items, totalCount) = await _customerRepository.GetListCustomerPagedAsync(page, pageSize);
            var dtoItems = _mapper.Map<List<CustomerResponse>>(items);
            return new PagedResponse<CustomerResponse> { Items = dtoItems, TotalCount = totalCount };
        }
    }
}
