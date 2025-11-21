namespace GuruPR.Application.Common.Interfaces.Application;

public interface IEmailTemplateService
{
    string BuildConfirmationEmailBody(string firstName, string confirmationLink);
}
