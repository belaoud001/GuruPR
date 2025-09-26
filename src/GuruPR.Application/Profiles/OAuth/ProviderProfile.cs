using AutoMapper;

using GuruPR.Application.Dtos.OAuth.Provider;
using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Profiles.OAuth;

public class ProviderProfile : Profile
{
    public ProviderProfile()
    {
        CreateMap<Provider, ProviderDto>();
        CreateMap<ProviderDto, Provider>();

        CreateMap<Provider, CreateProviderRequest>();
        CreateMap<CreateProviderRequest, Provider>()
            .ForMember(dest => dest.ProviderConnections, opt => opt.MapFrom(src => src.ProviderConnections));


        CreateMap<Provider, UpdateProviderRequest>();
        CreateMap<UpdateProviderRequest, Provider>();
    }
}
