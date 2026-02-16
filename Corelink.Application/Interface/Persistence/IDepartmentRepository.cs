using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IDepartmentRepository
{
    Task<Guid> CreateAsync(Department department);
    Task<Department?> GetByIdAsync(Guid id);
}