namespace App.Application.Features.Update;
public record UpdateProductsRequest(string Name, decimal Price, int Stock, int CategoryId);