using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Checkout;
using Corelink.Application.Interface.Persistence;

namespace Corelink.Application.Services;

public sealed class CheckoutService(ICheckoutRepository repository, ICartRepository cartRepository) : ICheckoutService
{
    public async Task<Answer<CartValidationResponse>> ValidateCartAsync(long userId)
    {
        var (response, _) = await ValidateAndGetItemsAsync(userId);
        return response;
    }
    
    public async Task<Answer<CheckoutResponse>> ProcessCheckoutAsync(long userId, CheckoutRequest request)
    {
        var (validationAnswer, items) = await ValidateAndGetItemsAsync(userId);
        
        if (validationAnswer.Response == null || !validationAnswer.Response.IsValid)
            return Answer<CheckoutResponse>.BadRequest(string.Join(", ", validationAnswer.Response?.Errors ?? new List<string>()));

        var cart = await cartRepository.GetCartByUserId(userId);
        if (cart == null)
            return Answer<CheckoutResponse>.BadRequest("No se encontró un carrito activo para pagar.");

        try
        {
            var orderId = await repository.ProcessCheckoutTransactionAsync(cart.Id, userId, validationAnswer.Response.Total, items!, request.PaymentMethodId, request.PaymentReference);
            return Answer<CheckoutResponse>.Ok(new CheckoutResponse { OrderId = orderId }, "Orden procesada exitosamente.");
        }
        catch (Exception ex)
        {
            return Answer<CheckoutResponse>.Error($"Error interno procesando el checkout: {ex.Message}");
        }
    }

    private async Task<(Answer<CartValidationResponse> Answer, IEnumerable<CartCheckoutItem>? Items)> ValidateAndGetItemsAsync(long userId)
    {
        var cart = await cartRepository.GetCartByUserId(userId);
        if (cart == null)
            return (Answer<CartValidationResponse>.BadRequest("No se encontró un carrito activo."), null);
            
        var status = await repository.GetCartStatusAsync(cart.Id, userId);
        if (status != "PENDING")
            return (Answer<CartValidationResponse>.BadRequest("El carrito no es válido o ya fue procesado."), null);

        var items = await repository.GetCartItemsForCheckoutAsync(cart.Id);
        
        var response = new CartValidationResponse { IsValid = true };
        
        if (!items.Any())
            return (Answer<CartValidationResponse>.BadRequest("El carrito está vacío."), null);

        foreach (var item in items)
        {
            if (item.ProductStatus != "ACTIVE" || item.BranchProductStatus != "ACTIVE")
            {
                response.IsValid = false;
                response.Errors.Add($"El producto {item.ProductName} ya no está activo.");
            }
            if (item.Quantity > item.Stock)
            {
                response.IsValid = false;
                response.Errors.Add($"El producto {item.ProductName} no tiene stock suficiente. Solo hay {item.Stock} disponibles.");
            }
            
            response.Subtotal += item.Price * item.Quantity;
        }

        response.Total = response.Subtotal;
        
        return (Answer<CartValidationResponse>.Ok(response), items);
    }
}
