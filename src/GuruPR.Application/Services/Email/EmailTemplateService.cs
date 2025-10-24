using System.Text.Encodings.Web;

using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GuruPR.Application.Services.Email;
public class EmailTemplateService : IEmailTemplateService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailTemplateService(LinkGenerator linkGenerator,
                              IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public string BuildConfirmationEmailBody(string firstName, string confirmationLink)
    {
        return $@"
                <div style=""font-family:'Inter', 'Noto Sans JP', Arial, sans-serif; color:#111; background-color:#f7f7f7; padding:40px 0;"">
                  <div style=""max-width:600px; margin:0 auto; background-color:#fff; padding:40px 50px; border:1px solid #e0e0e0;"">
    
                    <h1 style=""color:#111; font-weight:500; font-size:26px; margin-bottom:15px;"">
                      Welcome to <span style=""color:#1E40AF;"">GuruPR</span>
                    </h1>

                    <p style=""color:#333; font-size:16px; line-height:1.6; margin-bottom:25px;"">
                      Hi <strong>{HtmlEncoder.Default.Encode(firstName)}</strong>,
                    </p>

                    <p style=""color:#555; font-size:15px; line-height:1.6; margin-bottom:35px;"">
                      Thank you for joining <strong>GuruPR</strong>. Please confirm your email to activate your account and start exploring.
                    </p>

                    <div style=""text-align:center; margin-bottom:40px;"">
                      <a href=""{HtmlEncoder.Default.Encode(confirmationLink)}""
                         style=""background-color:#1E40AF; color:#fff; text-decoration:none; 
                                padding:12px 28px; font-weight:500; font-size:15px; 
                                display:inline-block;"">
                        Confirm Email
                      </a>
                    </div>

                    <p style=""color:#777; font-size:14px; line-height:1.6; margin-bottom:30px;"">
                      If you didn’t create an account with GuruPR, you can safely ignore this message.
                    </p>

                    <hr style=""border:none; border-top:1px solid #e0e0e0; margin:30px 0;"" />

                    <p style=""font-size:12px; color:#999; text-align:center;"">
                      © {DateTime.UtcNow.Year} GuruPR. All rights reserved.
                    </p>

                    <p style=""font-size:12px; color:#aaa; text-align:center;"">
                      Made with 💡 by the GuruPR team
                    </p>
                  </div>
                </div>";
    }
}
