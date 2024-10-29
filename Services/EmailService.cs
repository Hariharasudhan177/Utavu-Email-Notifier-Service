using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;

public class EmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ISecretProvider secretProvider, ILogger<EmailService> logger)
    {
        _logger = logger;

        try
        {
            _emailClient = new EmailClient(secretProvider.GetSecretAsync("EmailServiceConnectionString").Result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Email service constructor: {ex.Message}");
            throw; 
        }
    }

    public async Task SendEmailAsync(string userEmail, string subject, string plainTextContent, string htmlContent)
    {
        var content = new EmailContent(subject)
        {
            PlainText = plainTextContent,
            Html = htmlContent
        };

        var message = new EmailMessage(
            senderAddress: "DoNotReply@hirezzy.com",
            content: content,
            recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(userEmail) }));

        try
        {
            var response = await _emailClient.SendAsync(WaitUntil.Completed, message);
            _logger.LogInformation($"Email sent successfully with Message Id: {response.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
            throw; 
        }
    }
}
