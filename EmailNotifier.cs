using System;
using System.Threading.Tasks;
using Azure.Communication.Email;
using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace Utavu.EmailNotifierService
{
    public class EmailNotifier
    {
        private readonly ILogger<EmailNotifier> _logger;

        public EmailNotifier(ILogger<EmailNotifier> logger)
        {
            _logger = logger;
        }

        [Function(nameof(EmailNotifier))]
        public async Task Run([EventGridTrigger] LoginEvent loginEvent)
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}, Event data: {data}",
                loginEvent.Type, loginEvent.Subject, loginEvent.Data["Email"]);

            try
            {
                // Example email address; replace with the actual user's email from the event
                var emailTo = loginEvent.Data["Email"]?.ToString(); // Replace with extracted email

                // Send email notification
                await SendEmailNotification(emailTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in function execution: {ex.Message}");

            }
        }

        private async Task SendEmailNotification(string userEmail)
        {
            var subject = "Hirezzy - Login from new system";
            var plainTextContent = "You have logged in from a new device.";
            var htmlContent = "You have logged in from a new device.";

            // Create the email content and message
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
                var keyVaultUrl = new Uri("https://utavukv.vault.azure.net/");
                var client = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
                // Retrieve the connection string from Key Vault asynchronously
                var secret = await client.GetSecretAsync("EmailServiceConnectionString");
                var connectionString = secret.Value.Value;

                var _emailClient = new EmailClient(connectionString);
                var response = await _emailClient.SendAsync(WaitUntil.Completed, message);
                _logger.LogInformation($"Email sent successfully with Message Id: {response.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
        }
    }

    public class LoginEvent
    {
        public string Id { get; set; }

        public string Topic { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }

        public DateTime Time { get; set; }

        public IDictionary<string, object> Data { get; set; }
    }
}
