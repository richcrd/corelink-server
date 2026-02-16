using Corelink.Domain.Enums;

namespace Corelink.Application.Contracts.Departments;

public record DepartmentResponse(
    Guid Id,
    string Name
    );