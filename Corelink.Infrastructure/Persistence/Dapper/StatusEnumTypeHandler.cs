using System.Data;
using Corelink.Domain.Enums;
using Dapper;

namespace Corelink.Infrastructure.Persistence.Dapper;

internal sealed class StatusEnumTypeHandler : SqlMapper.TypeHandler<StatusEnum>
{
    public override void SetValue(IDbDataParameter parameter, StatusEnum value)
    {
        parameter.Value = value.ToDb();
        parameter.DbType = DbType.String;
    }

    public override StatusEnum Parse(object value)
    {
        return value switch
        {
            null => StatusEnum.ACTIVE,
            StatusEnum status => status,
            string s => StatusEnumExtensions.FromDb(s),
            _ => StatusEnumExtensions.FromDb(value.ToString() ?? string.Empty)
        };
    }
}
