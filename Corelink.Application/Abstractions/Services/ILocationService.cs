using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Locations;

namespace Corelink.Application.Abstractions.Services;

public interface ILocationService
{
    Task<Answer<LocationResponse>> CreateAsync(CreateLocationRequest request);
    Task<Answer<LocationResponse?>> GetByIdAsync(Guid id);
    Task<Answer<IReadOnlyList<LocationResponse>>> ListByDepartmentIdAsync(Guid departmentId);
}
