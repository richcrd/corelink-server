using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public StatusEnum Status { get; set; } = StatusEnum.Active;

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images;
    
    private readonly List<ProductBranch> _branches = new();
    public IReadOnlyCollection<ProductBranch> Branches => _branches;
    
    private Product() {}

    public Product(string name, string? description, Guid categoryId)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Status = StatusEnum.Active;
    }

    public void AddBranch(ProductBranch branch)
    {
        if (_branches.Any(b => b.BranchId == branch.BranchId))
            throw new InvalidOperationException("Product already exists in this branch");
        _branches.Add(branch);
    }

    public void AddImage(ProductImage image)
    {
        if (image.IsMain && _images.Any(i => i.IsMain))
            throw new InvalidOperationException("There is already a main image");
        _images.Add(image);
    }

    public void Deactivate()
    {
        Status = StatusEnum.Inactive;
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
}