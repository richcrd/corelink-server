using Corelink.Application.Contracts.Branches;
using Corelink.Domain.Entities;

namespace Corelink.Application.Mapper;

public static class BranchMapper
{
    public static BranchResponse ToResponse(Branch branch)
    {
        return new BranchResponse(
            branch.Id,
            branch.Name,
            branch.DepartmentName);
    }
}
