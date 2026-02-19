using Corelink.Application.Contracts.Locations;
using Corelink.Domain.Entities;

namespace Corelink.Application.Mapper;

public static class LocationMapper
{
    public static LocationResponse ToResponse(Location location)
    {
        return new LocationResponse(
            location.Id,
            location.Name,
            location.DepartmentName);
    }
}
