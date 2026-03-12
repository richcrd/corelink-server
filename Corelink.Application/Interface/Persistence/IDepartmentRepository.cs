using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IDepartmentRepository
{
    Task<long> CreateAsync(Department department);
    Task<Department?> GetByIdAsync(long id);
}