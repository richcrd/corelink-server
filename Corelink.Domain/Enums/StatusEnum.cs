namespace Corelink.Domain.Enums;

public enum StatusEnum
{
    Active,
    Inactive,
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public static class StatusEnumExtensions
{
    public static string ToDb(this StatusEnum status) => status.ToString().ToUpperInvariant();

    public static StatusEnum FromDb(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return StatusEnum.Active;
        }

        return Enum.Parse<StatusEnum>(value.Trim(), ignoreCase: true);
    }
}
