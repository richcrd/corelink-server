namespace Corelink.Application.Contracts.Branches;

public record BranchResponse(
    long Id,
    string Name,
    string DepartmentName
);
