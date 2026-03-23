namespace corelink_server.Models;

public class CreateProductCategoryModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public IFormFile Image { get; set; } = default!;
}
