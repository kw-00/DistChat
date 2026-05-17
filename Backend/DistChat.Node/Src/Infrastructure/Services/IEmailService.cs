using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MyApp.Infrastructure.Services;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    );
}