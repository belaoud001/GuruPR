using AutoMapper;

using GuruPR.Application.Features.Users.Dtos;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Features.Users.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.ToString()));
    }
}
