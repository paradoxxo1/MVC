using App.Application.Features.Products.Create;
using App.Application.Features.Products.Dto;
using App.Application.Features.Update;
using App.Domain.Entities;
using App.Domain.Entities.Common;
using AutoMapper;

namespace App.Services.Products;
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();

        CreateMap<CreateProductRequest, Product>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));

        CreateMap<UpdateProductsRequest, Product>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));
    }
}
