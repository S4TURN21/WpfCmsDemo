using AutoMapper;
using Remake.WebApi.Entities.DataTransferObjects;
using Remake.WebApi.Entities.Models;

namespace Remake.WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegistrationDto, User>().ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
