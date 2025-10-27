namespace GuruPR.Application.Interfaces.Application;

public interface IEmailTemplateService
{
    string BuildConfirmationEmailBody(string firstName, string confirmationLink);
}
