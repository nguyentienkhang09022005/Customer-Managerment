using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class CompanyMapper : Profile
    {
        public CompanyMapper() 
        {
            // Entity -> DTO
            CreateMap<Company, CompanyDTO>();


        }
    }
}
