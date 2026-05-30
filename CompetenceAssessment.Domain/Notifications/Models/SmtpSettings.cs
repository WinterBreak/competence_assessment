namespace CompetenceAssessment.Domain.Notifications;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public int WebPort { get; set; }
    public bool UseSsl { get; set; }
    public bool UseStartTls { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public bool EnableSslCertificateValidation { get; set; } = true;
}