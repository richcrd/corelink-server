namespace Corelink.Domain.Enums;

public enum StatusEnum
{
    ACTIVE,
    INACTIVE,
    PENDING,
    APROVVED,
    REJECTED,
    CANCELLED
}

public static class StatusEnumExtensions
{
    public static string ToDb(this StatusEnum status) => status.ToString().ToUpperInvariant();

    public static StatusEnum FromDb(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return StatusEnum.ACTIVE;
        }

        return Enum.Parse<StatusEnum>(value.Trim(), ignoreCase: true);
    }
}
