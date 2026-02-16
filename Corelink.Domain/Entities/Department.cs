using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; }
    public StatusEnum Status { get; set; } = StatusEnum.Active;
}