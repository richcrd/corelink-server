using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Branches;
using Corelink.Application.Contracts.Locations;

namespace Corelink.Application.Abstractions.Services;

public interface IBranchService
{
    Task<Answer<BranchResponse>> CreateAsync(CreateBranchRequest request);
    Task<Answer<BranchResponse?>> GetByIdAsync(long id);
    Task<Answer<IReadOnlyList<BranchResponse>>> ListByDepartmentIdAsync(long departmentId);
}
