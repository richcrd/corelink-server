using Corelink.Domain.Entities.Base;

namespace Corelink.Domain.Entities;

public class ProductImage : BaseEntity
{
    public string Url { get; set; }
    public bool IsMain { get; private set; }
    public int Position { get; private set; }

    public ProductImage(string url, bool isMain, int position)
    {
        Url = url;
        IsMain = isMain;
        Position = position;
    }

    public void SetAsMain()
    {
        IsMain = true;
    }
}