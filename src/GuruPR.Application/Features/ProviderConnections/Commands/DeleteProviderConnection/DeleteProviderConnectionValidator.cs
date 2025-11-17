using FluentValidation;

using GuruPR.Application.Features.ProviderConnections.Validators.Extensions;

namespace GuruPR.Application.Features.ProviderConnections.Commands.DeleteProviderConnection;

public class DeleteProviderConnectionValidator : AbstractValidator<DeleteProviderConnectionCommand>
{
    public DeleteProviderConnectionValidator()
    {
        RuleFor(command => command.ProviderConnectionId).ValidProviderConnectionId();
    }
}
