using CompetenceAssessment.Domain.Notifications;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CompetenceAssessment.Application.Notifications;

public class EmailNotificationService : IEmailNotificationService
{
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(
            IOptions<SmtpSettings> smtpSettings,
            ILogger<EmailNotificationService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailNotification notification)
        {
            try
            {
                using var email = new MimeMessage();
                
                email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                email.To.Add(new MailboxAddress(notification.ToName, notification.ToEmail));
                
                email.Subject = notification.Subject;
                
                var bodyBuilder = new BodyBuilder();
                if (notification.IsHtml)
                {
                    bodyBuilder.HtmlBody = notification.Body;
                }
                else
                {
                    bodyBuilder.TextBody = notification.Body;
                }
                
                
                email.Body = bodyBuilder.ToMessageBody();
                
                using var smtp = new SmtpClient();
                
                await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, GetSecureSocketOptions());
                
                if (!string.IsNullOrEmpty(_smtpSettings.Username))
                {
                    await smtp.AuthenticateAsync(
                        _smtpSettings.Username, 
                        _smtpSettings.Password);
                }
                
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to send email to {Email}", 
                    notification.ToEmail);
                throw;
            }
        }

        public async Task SendBulkEmailsAsync(IEnumerable<EmailNotification> notifications)
        {
            var tasks = notifications.Select(notification => 
                SendEmailAsync(notification));
            
            await Task.WhenAll(tasks);
        }

        public async Task SendTemplatedEmailAsync(
            string templateName, 
            Dictionary<string, string> templateData,
            string toEmail)
        {
            var body = await LoadTemplateAsync(templateName);
            
            foreach (var data in templateData)
            {
                body = body.Replace($"{{{{{data.Key}}}}}", data.Value);
            }
            
            var notification = new EmailNotification
            {
                ToEmail = toEmail,
                Subject = "Оценка компетенций",
                Body = body,
                IsHtml = true
            };
            
            await SendEmailAsync(notification);
        }

        private SecureSocketOptions GetSecureSocketOptions()
        {
            if (_smtpSettings.UseSsl)
                return SecureSocketOptions.SslOnConnect;
            
            if (_smtpSettings.UseStartTls)
                return SecureSocketOptions.StartTls;
            
            return SecureSocketOptions.None;
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(
                AppContext.BaseDirectory, 
                "Notifications",
                "Templates",
                $"{templateName}.html");
            
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    $"Email template {templateName} not found");
            }
            
            return await File.ReadAllTextAsync(templatePath);
        } }
