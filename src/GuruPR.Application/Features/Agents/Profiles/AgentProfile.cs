using AutoMapper;

using GuruPR.Application.Features.Agents.Commands.CreateAgent;
using GuruPR.Application.Features.Agents.Commands.UpdateAgent;
using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Domain.Entities.Agents;
using GuruPR.Domain.Entities.Agents.Operations;

namespace GuruPR.Application.Features.Agents.Profiles;

public class AgentProfile : Profile
{
    public AgentProfile()
    {
        CreateMap<Agent, AgentDto>();
        CreateMap<AgentDto, Agent>();

        CreateMap<CreateAgentCommand, Agent>();
        CreateMap<Agent, CreateAgentCommand>();

        CreateMap<UpdateAgentCommand, AgentUpdateData>();
        CreateMap<AgentUpdateData, UpdateAgentCommand>();
    }
}
