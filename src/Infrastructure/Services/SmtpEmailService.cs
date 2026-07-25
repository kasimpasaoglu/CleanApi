#pragma warning disable CA1873
using System.Net;
using System.Net.Mail;
using Infrastructure.Services.Models;

namespace Infrastructure.Services;

public class SmtpEmailService(
    IOptions<SmtpSettings> settings,
    ILogger<SmtpEmailService> logger) : ISmtpEmailService
{
    private readonly SmtpSettings _settings = settings.Value;

    public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            using var smtp = CreateClient();

            await smtp.SendMailAsync(message);

            logger.LogInformation("Email sent to {Recipient} with subject {Subject}", to, subject);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending email To:{to}, Subject:{subject}", to, subject);
            throw;
        }
    }


    public async Task SendBulkBccAsync(
        IReadOnlyCollection<string> to,
        string subject,
        string body,
        bool isHtml = true)
    {
        try
        {
            if (to.Count == 0)
                throw new ArgumentException("Alıcı listesi boş olamaz.", nameof(to));

            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var recipient in to.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
            {
                message.Bcc.Add(recipient);
            }

            if (message.Bcc.Count == 0)
                throw new ArgumentException("Geçerli alıcı bulunamadı.", nameof(to));

            using var smtp = CreateClient();
            await smtp.SendMailAsync(message);
            logger.LogInformation("Bulk email sent. Count: {Count}, Subject: {Subject}", message.Bcc.Count, subject);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending bulk email. Count:{Count}, Subject:{Subject}", to.Count, subject);
            throw;
        }
    }


    public async Task SendWithBccAsync(
        string to,
        IReadOnlyCollection<string> bcc,
        string subject,
        string body,
        bool isHtml = true)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Birincil alıcı boş olamaz.", nameof(to));

            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            foreach (var recipient in bcc.Where(x => !string.IsNullOrWhiteSpace(x) &&
                                                    !string.Equals(x, to, StringComparison.OrdinalIgnoreCase))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                message.Bcc.Add(recipient);
            }

            using var smtp = CreateClient();
            await smtp.SendMailAsync(message);
            logger.LogInformation(
                "Email sent to {Recipient} with {BccCount} BCC. Subject: {Subject}",
                to, message.Bcc.Count, subject);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending email To:{To}, BccCount:{BccCount}, Subject:{Subject}", to, bcc.Count, subject);
            throw;
        }
    }


    /// <summary>
    /// E-mail istemcisini olusturan helper metod
    /// </summary>
    /// <returns>STMP istemcisi</returns>
    private SmtpClient CreateClient()
    {
        return new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.UseSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };
    }
}