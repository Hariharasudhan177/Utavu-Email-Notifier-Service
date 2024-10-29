public interface IEmailService
{
    Task SendEmailAsync(string userEmail, string subject, string plainTextContent, string htmlContent);
}