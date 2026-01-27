using App.Repositories;
using App.Repositories.Products;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products;
public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IValidator<CreateProductRequest> createProductRequestValidator) : IProductService
{
    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductAsync(count);

        var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return new ServiceResult<List<ProductDto>>()
        {
            Data = productsAsDto
        };
    }

    public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
    {
        var products = await productRepository.GetAll().ToListAsync();

        var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPageAllListAsync(int pageNumber, int pageSize)
    {

        var prdoucts = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var productsAsDto = prdoucts.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            ServiceResult<ProductDto>.Fail("Prodcut not found", HttpStatusCode.NotFound);
        }

        var productAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);

        return ServiceResult<ProductDto>.Success(productAsDto)!;
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        // asenkron validation manuel bussines kontrol

        var anyProduct = await productRepository.Where(x => x.Name == request.Name).AnyAsync();

        if (anyProduct)
        {
            return ServiceResult<CreateProductResponse>.Fail("Ürün ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
        }

        #region asenkron validation manuel fluent validation bussines kontrol
        //
        //var validationResult = await createProductRequestValidator.ValidateAsync(request);

        //if (!validationResult.IsValid)
        //{
        //    return ServiceResult<CreateProductResponse>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        //}
        #endregion

        var product = new Product()
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
        };

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangeAsync();
        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductsRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Fail("Product Not Found", HttpStatusCode.NotFound);
        }
        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        productRepository.Update(product);
        await unitOfWork.SaveChangeAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
        {
            return ServiceResult.Fail("Product Not Found", HttpStatusCode.NotFound);
        }

        product.Stock = request.Quantity;

        productRepository.Update(product);
        await unitOfWork.SaveChangeAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return ServiceResult.Fail("Product Not Found", HttpStatusCode.BadRequest);
        }

        productRepository.Delete(product);
        await unitOfWork.SaveChangeAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
