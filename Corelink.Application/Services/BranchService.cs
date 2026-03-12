using Corelink.Application.Abstractions.Services;
using Corelink.Application.Common;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Branches;
using Corelink.Application.Contracts.Locations;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;

namespace Corelink.Application.Services;

public sealed class BranchService(
    IBranchRepository branchRepository,
    IDepartmentRepository departmentRepository) : IBranchService
{
    private static string NormalizeName(string name) => Validation.Trim(name);

    public async Task<Answer<BranchResponse>> CreateAsync(CreateBranchRequest request)
    {
        var error = Validation.FirstError(
            Validation.Required<BranchResponse>(request.Name, nameof(request.Name)),
            Validation.RequiredLong<BranchResponse>(request.DepartmentId, nameof(request.DepartmentId)));

        if (error is not null) return error;

        var name = NormalizeName(request.Name!);

        var department = await departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
        {
            return Answer<BranchResponse>.NotFound("Department not found");
        }

        if (await branchRepository.ExistsByNameInDepartmentAsync(name, request.DepartmentId))
        {
            return Answer<BranchResponse>.BadRequest("Branch already exists in department");
        }

        var branch = new Branch
        {
            Name = name,
            DepartmentId = request.DepartmentId,
            Status = StatusEnum.ACTIVE
        };

        var id = await branchRepository.CreateAsync(branch);
        branch.Id = id;

        return Answer<BranchResponse>.Ok(BranchMapper.ToResponse(branch), "Branch created");
    }

    public async Task<Answer<BranchResponse?>> GetByIdAsync(long id)
    {
        if (id <= 0)
        {
            return Answer<BranchResponse?>.BadRequest("Id is required");
        }

        var branch = await branchRepository.GetByIdAsync(id);
        return branch is null
            ? Answer<BranchResponse?>.NotFound("Branch not found")
            : Answer<BranchResponse?>.Ok(BranchMapper.ToResponse(branch));
    }

    public async Task<Answer<IReadOnlyList<BranchResponse>>> ListByDepartmentIdAsync(long departmentId)
    {
        if (departmentId <= 0)
        {
            return Answer<IReadOnlyList<BranchResponse>>.BadRequest("DepartmentId is required");
        }

        var department = await departmentRepository.GetByIdAsync(departmentId);
        if (department is null)
        {
            return Answer<IReadOnlyList<BranchResponse>>.NotFound("Department not found");
        }

        var branches = await branchRepository.ListByDepartmentIdAsync(departmentId);
        var response = branches.Select(BranchMapper.ToResponse).ToList();

        return Answer<IReadOnlyList<BranchResponse>>.Ok(response);
    }
}
