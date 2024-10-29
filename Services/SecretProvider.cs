using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class SecretProvider : ISecretProvider
{
    private readonly ILogger<SecretProvider> _logger;
    private readonly IConfiguration _configuration;
    private readonly SecretClient _secretClient;

    public SecretProvider(IConfiguration configuration, ILogger<SecretProvider> logger)
    {
        _logger = logger;

        try
        {
            _configuration = configuration;

            var keyVaultUrl = configuration["KeyVaultUrl"];

            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Secret provider constructor: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value.Value;
    }
}