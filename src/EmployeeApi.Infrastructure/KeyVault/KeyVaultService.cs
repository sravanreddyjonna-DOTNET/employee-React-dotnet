using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EmployeeApi.Application.Abstractions.Services;

namespace EmployeeApi.Infrastructure.KeyVault;

public sealed class KeyVaultService : IKeyVaultService
{
    private readonly SecretClient _client;

    public KeyVaultService(string vaultUrl)
    {
        _client = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());
    }

    public async Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetSecretAsync(secretName, cancellationToken: cancellationToken);
        return response.Value.Value;
    }
}
