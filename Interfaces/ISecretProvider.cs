public interface ISecretProvider
{
    Task<string> GetSecretAsync(string secretName);
}