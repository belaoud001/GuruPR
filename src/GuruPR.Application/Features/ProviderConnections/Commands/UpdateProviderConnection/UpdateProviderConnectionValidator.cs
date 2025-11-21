using FluentValidation;

using GuruPR.Application.Features.ProviderConnections.Validators.Extensions;

namespace GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;

public class UpdateProviderConnectionValidator : AbstractValidator<UpdateProviderConnectionCommand>
{
    public UpdateProviderConnectionValidator()
    {
        RuleFor(command => command.Id!).ValidProviderConnectionId();

        RuleFor(command => command.ClientId).ValidClientId();

        RuleFor(command => command.ClientSecret).ValidClientSecret();

        RuleFor(command => command.Scopes).ValidScopes();
    }
}
