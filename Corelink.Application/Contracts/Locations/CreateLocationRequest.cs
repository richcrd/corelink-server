namespace Corelink.Application.Contracts.Locations;

public record CreateLocationRequest(
    string Name,
    Guid DepartmentId
);
