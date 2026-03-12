using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public required string Name { get; set; }
    public string Description { get; set; }
    public StatusEnum Status { get; set; } = StatusEnum.ACTIVE;
}