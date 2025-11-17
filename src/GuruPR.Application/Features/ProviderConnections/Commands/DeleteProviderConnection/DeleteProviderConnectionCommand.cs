using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Commands.DeleteProviderConnection;

public record DeleteProviderConnectionCommand(string ProviderConnectionId) : IRequest;
