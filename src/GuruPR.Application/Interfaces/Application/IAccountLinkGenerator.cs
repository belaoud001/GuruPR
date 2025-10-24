namespace GuruPR.Application.Interfaces.Application;

public interface IAccountLinkGenerator
{
    string GenerateConfirmationLink(Guid userId, string token);
}
