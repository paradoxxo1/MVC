using App.Application.Features.Products.Create;
using App.Application.Features.Products.Dto;
using App.Application.Features.Update;
using App.Application.Features.UpdateStock;

namespace App.Application.Features.Products;
public interface IProductService
{
    Task<ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count);                                                                                                                                                   
    Task<ServiceResult<ProductDto?>> GetByIdAsync(int id);
    Task<ServiceResult<List<ProductDto>>> GetAllListAsync();
    Task<ServiceResult<List<ProductDto>>> GetPageAllListAsync(int pageNumber, int pageSize);
    Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateProductsRequest request);
    Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request);
    Task<ServiceResult> DeleteAsync(int id);
}
