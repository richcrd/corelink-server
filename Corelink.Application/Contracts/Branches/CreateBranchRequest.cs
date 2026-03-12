namespace Corelink.Application.Contracts.Locations;

public record CreateBranchRequest(
    string Name,
    long DepartmentId
);
