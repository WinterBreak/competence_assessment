using CompetenceAssessment.Domain.Notifications;

namespace CompetenceAssessment.Domain.Notifications;

public interface IEmailNotificationService
{
    Task SendEmailAsync(EmailNotification notification);
    
    Task SendBulkEmailsAsync(IEnumerable<EmailNotification> notifications);
    
    Task SendTemplatedEmailAsync(string templateName, Dictionary<string, string> templateData, 
        string toEmail);
}