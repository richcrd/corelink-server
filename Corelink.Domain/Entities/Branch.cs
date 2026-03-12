using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Address { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public Department Department { get; set; } = null!;
    public StatusEnum Status { get; set; } = StatusEnum.ACTIVE;
}