using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Utavu.EmailNotifierService;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<ISecretProvider, SecretProvider>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<EmailNotifier>();
    })
    .Build();

host.Run();