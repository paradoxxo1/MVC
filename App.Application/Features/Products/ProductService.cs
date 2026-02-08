using App.Application.Contracts.Persistence;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Dto;
using App.Application.Features.Update;
using App.Application.Features.UpdateStock;
using App.Domain.Entities;
using App.Domain.Entities.Common;
using AutoMapper;
using FluentValidation;
using System.Net;

namespace App.Application.Features.Products;
public class ProductService(IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateProductRequest> createProductRequestValidator,
    IMapper mapper) : IProductService
{
    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductAsync(count);

        var productsAsDto = mapper.Map<List<ProductDto>>(products);

        return new ServiceResult<List<ProductDto>>()
        {
            Data = productsAsDto
        };
    }

    public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
    {
        var products = await productRepository.GetAllAsync();

        #region Manuel Mapping

        //var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        #endregion

        var productAsDto = mapper.Map<List<ProductDto>>(products);

        return ServiceResult<List<ProductDto>>.Success(productAsDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPageAllListAsync(int pageNumber, int pageSize)
    {

        var products = await productRepository.GetAllPagedAsync(pageNumber, pageSize);

        #region Manuel Mapping
        //var productsAsDto = prdoucts.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
        #endregion

        var productAsDto = mapper.Map<List<ProductDto>>(products);


        return ServiceResult<List<ProductDto>>.Success(productAsDto);
    }

    public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return ServiceResult<ProductDto?>.Fail("Prodcut not found", HttpStatusCode.NotFound);
        }

        #region Manuel Mapping
        //var productAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
        #endregion

        var productAsDto = mapper.Map<ProductDto>(product);


        return ServiceResult<ProductDto>.Success(productAsDto)!;
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        //throw new CriticalException("kritik hata");
        //throw new Exception("denemelik hata");
        // asenkron validation manuel bussines kontrol

        var anyProduct = await productRepository.AnyAsync(x => x.Name == request.Name);

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

        var product = mapper.Map<Product>(request);

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangeAsync();
        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductsRequest request)
    {


        var isProductNameExist = await productRepository.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (isProductNameExist)
        {
            return ServiceResult.Fail("ürün ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
        }

        //product.Name = request.Name;
        //product.Price = request.Price;
        //product.Stock = request.Stock;

        var product = mapper.Map<Product>(request);
        product.Id = id;

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

        productRepository.Delete(product!);
        await unitOfWork.SaveChangeAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
