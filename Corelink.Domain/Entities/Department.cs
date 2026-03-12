using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = null!;
    public StatusEnum Status { get; set; } = StatusEnum.ACTIVE;
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}