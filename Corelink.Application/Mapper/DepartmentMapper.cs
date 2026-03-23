using Corelink.Application.Contracts.Departments;
using Corelink.Domain.Entities;

namespace Corelink.Application.Mapper;

public static class DepartmentMapper
{
    public static DepartmentResponse ToResponse(Department department)
    {
        return new DepartmentResponse(
            department.Id,
            department.Name);
    }
}