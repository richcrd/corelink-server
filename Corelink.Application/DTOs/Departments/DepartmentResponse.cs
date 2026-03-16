using Corelink.Domain.Enums;

namespace Corelink.Application.Contracts.Departments;

public record DepartmentResponse(
    long Id,
    string Name
    );