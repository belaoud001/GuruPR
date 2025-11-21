using GuruPR.Application.Common.Settings.Email;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

using MimeKit;

namespace GuruPR.Infrastructure.Services.Email;

public class GmailSender : IEmailSender
{
    private GmailingAppSettings _gmailingSettings;

    public GmailSender(IOptions<GmailingAppSettings> gmailingSettings)
    {
        _gmailingSettings = gmailingSettings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(_gmailingSettings.Username));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = htmlBody
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(_gmailingSettings.Host, _gmailingSettings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_gmailingSettings.Username, _gmailingSettings.AppPassword);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}
