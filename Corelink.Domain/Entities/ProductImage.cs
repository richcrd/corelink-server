using Corelink.Domain.Entities.Base;

namespace Corelink.Domain.Entities;

public class ProductImage : BaseEntity
{
    public string Url { get; set; }
    public bool IsMain { get; private set; }
    public int Position { get; private set; }
    
    private ProductImage() { }

    public ProductImage(Guid id, string url, bool isMain, int position)
    {
        Id = id;
        Url = url;
        IsMain = isMain;
        Position = position;
    }

    public void SetAsMain()
    {
        IsMain = true;
    }
}