using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Departments;

namespace Corelink.Application.Abstractions.Services;

public interface IDepartmentService
{
    Task<Answer<DepartmentResponse?>> GetByIdAsync(long id);
}