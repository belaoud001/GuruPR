using AutoMapper;

using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Dtos.OAuth.ProviderConnection;

namespace GuruPR.Application.Profiles.OAuth;

public class ProviderConnectionProfile : Profile
{
    public ProviderConnectionProfile()
    {
        CreateMap<ProviderConnection, ProviderConnectionDto>();
        CreateMap<ProviderConnectionDto, ProviderConnection>();

        CreateMap<ProviderConnection, CreateProviderConnectionRequest>();
        CreateMap<CreateProviderConnectionRequest, ProviderConnection>();

        CreateMap<ProviderConnection, UpdateProviderConnectionRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateProviderConnectionRequest, ProviderConnection>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
