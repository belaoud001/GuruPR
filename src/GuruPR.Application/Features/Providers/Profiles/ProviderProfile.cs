using AutoMapper;

using GuruPR.Application.Features.Agents.Commands.CreateAgent;
using GuruPR.Application.Features.Providers.Commands.UpdateProvider;
using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Domain.Entities.Provider;
using GuruPR.Domain.Entities.Provider.Operations;

namespace GuruPR.Application.Features.Providers.Profiles;

public class ProviderProfile : Profile
{
    public ProviderProfile()
    {
        CreateMap<Provider, Provider>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Provider, ProviderDto>();
        CreateMap<ProviderDto, Provider>();

        CreateMap<Provider, CreateAgentCommand>();
        CreateMap<CreateAgentCommand, Provider>();

        CreateMap<ProviderUpdateData, UpdateProviderCommand>();
        CreateMap<UpdateProviderCommand, ProviderUpdateData>();
    }
}
