using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductDomain>();

            CreateMap<ProductDomain, Product>()
                .ForMember(dest => dest.IdProduct, opt => opt.Ignore());

            CreateMap<ProductDomain, ProductResponse>();

            CreateMap<Product, ProductResponse>();

            CreateMap<ProductCreationRequest, ProductDomain>();

            CreateMap<ProductUpdateRequest, ProductDomain>();

        }
    }
}
