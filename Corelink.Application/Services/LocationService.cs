using Corelink.Application.Abstractions.Services;
using Corelink.Application.Common;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Locations;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;

namespace Corelink.Application.Services;

public sealed class LocationService(
    ILocationRepository locationRepository,
    IDepartmentRepository departmentRepository) : ILocationService
{
    private static string NormalizeName(string name) => Validation.Trim(name);

    public async Task<Answer<LocationResponse>> CreateAsync(CreateLocationRequest request)
    {
        var error = Validation.FirstError(
            Validation.Required<LocationResponse>(request.Name, nameof(request.Name)),
            Validation.RequiredGuid<LocationResponse>(request.DepartmentId, nameof(request.DepartmentId)));

        if (error is not null) return error;

        var name = NormalizeName(request.Name!);

        var department = await departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
        {
            return Answer<LocationResponse>.NotFound("Department not found");
        }

        if (await locationRepository.ExistsByNameInDepartmentAsync(name, request.DepartmentId))
        {
            return Answer<LocationResponse>.BadRequest("Location already exists in department");
        }

        var location = new Location
        {
            Name = name,
            DepartmentId = request.DepartmentId,
            Status = StatusEnum.Active
        };

        var id = await locationRepository.CreateAsync(location);
        location.Id = id;

        return Answer<LocationResponse>.Ok(LocationMapper.ToResponse(location), "Location created");
    }

    public async Task<Answer<LocationResponse?>> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Answer<LocationResponse?>.BadRequest("Id is required");
        }

        var location = await locationRepository.GetByIdAsync(id);
        return location is null
            ? Answer<LocationResponse?>.NotFound("Location not found")
            : Answer<LocationResponse?>.Ok(LocationMapper.ToResponse(location));
    }

    public async Task<Answer<IReadOnlyList<LocationResponse>>> ListByDepartmentIdAsync(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
        {
            return Answer<IReadOnlyList<LocationResponse>>.BadRequest("DepartmentId is required");
        }

        var department = await departmentRepository.GetByIdAsync(departmentId);
        if (department is null)
        {
            return Answer<IReadOnlyList<LocationResponse>>.NotFound("Department not found");
        }

        var locations = await locationRepository.ListByDepartmentIdAsync(departmentId);
        var response = locations.Select(LocationMapper.ToResponse).ToList();

        return Answer<IReadOnlyList<LocationResponse>>.Ok(response);
    }
}
