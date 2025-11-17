using FluentValidation;

using GuruPR.Application.Features.ProviderConnections.Validators.Extensions;

namespace GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;

public class CreateProviderConnectionValidator : AbstractValidator<CreateProviderConnectionCommand>
{
    public CreateProviderConnectionValidator()
    {
        RuleFor(providerConnection => providerConnection.ClientId).ValidClientId();

        RuleFor(providerConnection => providerConnection.ClientSecret).ValidClientSecret();

        RuleFor(providerConnection => providerConnection.Scopes).ValidScopes();
    }
}
