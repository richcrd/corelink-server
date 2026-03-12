using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class Person : BaseEntity
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string? PhoneNumber { get; set; }
	public string? Address { get; set; }
	public long BranchId { get; set; }
	public StatusEnum Status { get; set; } = StatusEnum.ACTIVE;
}