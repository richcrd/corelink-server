using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Departments;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;

namespace Corelink.Application.Services;

public class DepartmentService(IDepartmentRepository repository) : IDepartmentService
{
    public async Task<Answer<DepartmentResponse?>> GetByIdAsync(Guid id)
    {
        var department = await repository.GetByIdAsync(id);
        return department is null
            ? Answer<DepartmentResponse?>.NotFound("Department not found")
            : Answer<DepartmentResponse?>.Ok(DepartmentMapper.ToResponse(department));
    }
}