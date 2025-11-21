using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Agents.Exceptions;

public class AgentNotFoundException(string message) : NotFoundException(message);
