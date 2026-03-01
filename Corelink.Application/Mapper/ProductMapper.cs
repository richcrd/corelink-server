using Corelink.Application.Contracts.Products;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;

namespace Corelink.Application.Mapper;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.CategoryId,
            product.Status.ToString(),
            product.Images.Select(ToImageResponse).ToList(),
            product.Branches.Select(ToBranchResponse).ToList()
            );
    }

    private static ProductImageResponse ToImageResponse(ProductImage image)
    {
        return new ProductImageResponse(
            image.Id,
            image.Url,
            image.IsMain,
            image.Position
            );
    }

    private static ProductBranchResponse ToBranchResponse(ProductBranch branch)
    {
        return new ProductBranchResponse(
            branch.Id,
            branch.BranchId,
            branch.Price,
            branch.GetFinalPrice(),
            branch.Status.ToString(),
            branch.Offers.Select(ToOfferResponse).ToList()
            );
    }

    private static ProductOfferResponse ToOfferResponse(ProductOffer offer)
    {
        return new ProductOfferResponse(
            offer.Id,
            offer.OfferPrice,
            offer.StartDate,
            offer.EndDate,
            offer.Status.ToString(),
            offer.IsActive()
        );
    }
}