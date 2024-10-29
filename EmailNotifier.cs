using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Utavu.EmailNotifierService
{
    public class EmailNotifier
    {
        private readonly ILogger<EmailNotifier> _logger;
        private readonly IEmailService _emailService;

        public EmailNotifier(ILogger<EmailNotifier> logger, IEmailService emailService)
        {
            _logger = logger;

            try
            {
                _emailService = emailService;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, $"Email notifier constructor: {ex.Message}");
                throw; 
            }
        }

        [Function(nameof(EmailNotifier))]
        public async Task Run([EventGridTrigger] LoginEvent loginEvent)
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}, Event data: {data}",
                loginEvent.Type, loginEvent.Subject, loginEvent.Data["Email"]);

            try
            {
                var emailTo = loginEvent.Data["Email"]?.ToString();

                await _emailService.SendEmailAsync(emailTo,  
                    "Hirezzy - Login from new system",
                    "You have logged in from a new device.", 
                    "<p>You have logged in from a new device.</p>");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in function execution: {ex.Message}");

            }
        }
    }
}
