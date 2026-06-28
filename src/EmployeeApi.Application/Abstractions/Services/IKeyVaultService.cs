namespace EmployeeApi.Application.Abstractions.Services;

public interface IKeyVaultService
{
    Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);
}
