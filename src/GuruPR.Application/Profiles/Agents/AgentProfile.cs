using AutoMapper;

using GuruPR.Application.Dtos.Agent;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Profiles.Agents;

public class AgentProfile : Profile
{
    public AgentProfile()
    {
        CreateMap<Agent, AgentDto>();
        CreateMap<AgentDto, Agent>();

        CreateMap<CreateAgentRequest, Agent>();
        CreateMap<Agent, CreateAgentRequest>();

        CreateMap<UpdateAgentRequest, Agent>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Agent, UpdateAgentRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    }
}
