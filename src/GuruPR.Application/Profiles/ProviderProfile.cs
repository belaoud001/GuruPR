using AutoMapper;

using GuruPR.Application.Dtos.OAuth;
using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Profiles;

public class ProviderProfile : Profile
{
    public ProviderProfile()
    {
        CreateMap<Provider, ProviderDto>();
        CreateMap<ProviderDto, Provider>();

        CreateMap<Provider, CreateProviderRequest>();
        CreateMap<CreateProviderRequest, Provider>();
    }
}
