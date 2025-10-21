using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            // User <-> UserDomain
            CreateMap<User, UserDomain>().ReverseMap();

            // UserDomain -> UserResponse
            CreateMap<UserDomain, UserResponse>();

            // User -> UserResponse
            CreateMap<User, UserResponse>();

            // UserRequest -> UserDomain
        }
    }
}
