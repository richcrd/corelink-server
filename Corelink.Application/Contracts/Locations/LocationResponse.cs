namespace Corelink.Application.Contracts.Locations;

public record LocationResponse(
    Guid Id,
    string Name,
    string DepartmentName
);
