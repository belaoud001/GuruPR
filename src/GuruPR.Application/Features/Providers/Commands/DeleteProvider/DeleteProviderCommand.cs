using MediatR;

namespace GuruPR.Application.Features.Providers.Commands.DeleteProvider;

public record DeleteProviderCommand(string Id) : IRequest;
