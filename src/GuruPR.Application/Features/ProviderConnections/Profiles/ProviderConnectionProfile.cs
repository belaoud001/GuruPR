using AutoMapper;

using GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Dtos;
using GuruPR.Domain.Entities.ProviderConnection;

namespace GuruPR.Application.Features.ProviderConnections.Profiles;

public class ProviderConnectionProfile : Profile
{
    public ProviderConnectionProfile()
    {
        CreateMap<ProviderConnection, ProviderConnectionDto>();
        CreateMap<ProviderConnectionDto, ProviderConnection>();

        CreateMap<ProviderConnection, CreateProviderConnectionCommand>();
        CreateMap<CreateProviderConnectionCommand, ProviderConnection>();

        CreateMap<ProviderConnection, UpdateProviderConnectionCommand>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateProviderConnectionCommand, ProviderConnection>();
    }
}
