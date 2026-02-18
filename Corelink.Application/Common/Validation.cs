using Corelink.Application.Contracts;

namespace Corelink.Application.Common;

public static class Validation
{
    public static Answer<T>? Required<T>(string? value, string fieldName)
        => string.IsNullOrWhiteSpace(value) ? Answer<T>.BadRequest($"{fieldName} is required") : null;

    public static Answer<T>? RequiredGuid<T>(Guid value, string fieldName)
        => value == Guid.Empty ? Answer<T>.BadRequest($"{fieldName} is required") : null;

    public static Answer<T>? Ensure<T>(bool condition, string message)
        => condition ? null : Answer<T>.BadRequest(message);

    public static Answer<T>? FirstError<T>(params Answer<T>?[] errors)
        => errors.FirstOrDefault(e => e is not null);

    public static string Trim(string value) => value.Trim();

    public static string? TrimToNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
