using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IBranchRepository
{
	Task<long> CreateAsync(Branch branch);
	Task<Branch?> GetByIdAsync(long id);
	Task<IReadOnlyList<Branch>> ListByDepartmentIdAsync(long departmentId);
	Task<bool> ExistsByNameInDepartmentAsync(string name, long departmentId);
}