namespace Corelink.Application.Contracts.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string DefaultRoleName { get; set; } = "Customer";
}
