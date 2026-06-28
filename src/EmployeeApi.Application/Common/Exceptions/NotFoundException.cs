namespace EmployeeApi.Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested entity doesn't exist. Caught by the API's
/// exception-handling middleware and translated into a 404 response.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}
