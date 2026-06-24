using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MyApp.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(EmailOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(_options.FromName, _options.FromEmail));

        message.To.Add(MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtp.AuthenticateAsync(
            _options.Username,
            _options.Password,
            cancellationToken);

        await smtp.SendAsync(message, cancellationToken);

        await smtp.DisconnectAsync(true, cancellationToken);
    }
}