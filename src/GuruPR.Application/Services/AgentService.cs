using AutoMapper;

using GuruPR.Application.Dtos.Agent;
using GuruPR.Application.Exceptions;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Exceptions.Agent;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Entities.Configurations.Enums;
using GuruPR.Domain.Errors;
using GuruPR.Domain.Requests;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Services;

public class AgentService : IAgentService
{
    private readonly ILogger<AgentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public AgentService(ILogger<AgentService> logger,
                        IUnitOfWork unitOfWork,
                        IMapper mapper,
                        UserManager<User> userManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    #region Public Methods

    public async Task<IEnumerable<Agent>> GetAllAgentsAsync(string? userId = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return await _unitOfWork.Agents.GetAllAsync();
        }

        return await _unitOfWork.Agents.GetAllAgentsAsync(userId);
    }

    public async Task<Agent> GetAgentByIdAsync(string agentId)
    {
        var agent = await _unitOfWork.Agents.GetByIdAsync(agentId);
        if (agent == null)
        {
            throw new NotFoundException("Agent with ID {agentId} not found.");
        }

        return agent;
    }

    public async Task<Agent> CreateAgentAsync(CreateAgentRequest createAgentRequest, string userId)
    {
        var user = await GetUserByIdOrThrowException(userId);
        var agent = _mapper.Map<Agent>(createAgentRequest);

        agent.CreatedByUserId = user.Id.ToString();

        ValidateAgent(agent);

        var createdAgent = await _unitOfWork.Agents.AddAsync(agent);
        await _unitOfWork.SaveGuruChangesAsync();

        return createdAgent;
    }

    public async Task<Agent> UpdateAgentAsync(string agentId, UpdateAgentRequest updateAgentRequest)
    {
        var agent = await GetAgentByIdAsync(agentId);
        var updatedAgent = _mapper.Map(updateAgentRequest, agent);

        ValidateAgent(updatedAgent);

        _unitOfWork.Agents.Update(updatedAgent);
        await _unitOfWork.SaveGuruChangesAsync();

        return updatedAgent;
    }

    public async Task<bool> DeleteAgentAsync(string agentId)
    {
        var agent = await GetAgentByIdAsync(agentId);

        _unitOfWork.Agents.Delete(agent);

        var result = await _unitOfWork.SaveGuruChangesAsync();
        return result > 0;
    }

    #endregion

    #region Private Methods

    private void ValidateAgent(Agent agent)
    {
        var validationErrors = agent.Validate().ToList();
        if (validationErrors.Any())
        {
            throw CreateValidationException(validationErrors);
        }
    }

    private AgentValidationException CreateValidationException(IEnumerable<ValidationError> errors)
    {
        var errorGroups = errors.GroupBy(error => error.Field)
                                .ToDictionary(
                                    group => group.Key,
                                    group => group.Select(error => error.Message)
                                                  .ToList()
                                );

        return new AgentValidationException($"Agent validation failed.", errorGroups);
    }

    private async Task<User> GetUserByIdOrThrowException(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException($"User with the specified ID {userId} was not found.");
        }

        return user;
    }

    #endregion
}
