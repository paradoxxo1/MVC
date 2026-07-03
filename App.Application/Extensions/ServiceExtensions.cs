using App.Application.Features.Products;
using App.Services.Categories;
using App.Services.Products;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace App.Application.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<Features.Categories.ICategoryService, CategoryService>();
        //services.AddScoped(typeof(NotFoundFiler<,>));

        //services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        //services.AddExceptionHandler<CriticalExceptionHandler>();
        //services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
