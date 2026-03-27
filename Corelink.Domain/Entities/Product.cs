using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public long CategoryId { get; set; }
    public StatusEnum Status { get; set; } = StatusEnum.ACTIVE;

    
    private readonly List<ProductBranch> _branches = new();
    public IReadOnlyCollection<ProductBranch> Branches => _branches;
    
    private Product() { Name = string.Empty; }

    public Product(string name, string? description, long categoryId)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Status = StatusEnum.ACTIVE;
    }

    public void AddBranch(ProductBranch branch)
    {
        if (_branches.Any(b => b.BranchId == branch.BranchId))
            throw new InvalidOperationException("Product already exists in this branch");
        _branches.Add(branch);
    }


    public void Deactivate()
    {
        Status = StatusEnum.INACTIVE;
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        Name = name.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }

    public string? ImageUrl { get; private set; }
    public long? ImageId { get; private set; }

    public void SetImage(long imageId, string imageUrl)
    {
        ImageId = imageId;
        ImageUrl = imageUrl;
    }
}