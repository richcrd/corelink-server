using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface ILocationRepository
{
	Task<Guid> CreateAsync(Location location);
	Task<Location?> GetByIdAsync(Guid id);
	Task<IReadOnlyList<Location>> ListByDepartmentIdAsync(Guid departmentId);
	Task<bool> ExistsByNameInDepartmentAsync(string name, Guid departmentId);
}