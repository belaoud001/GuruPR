namespace GuruPR.Application.Common.Interfaces.Application;

public interface IAccountLinkGenerator
{
    string GenerateConfirmationLink(Guid userId, string token);
}
