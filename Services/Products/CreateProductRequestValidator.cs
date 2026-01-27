using App.Repositories.Products;
using FluentValidation;

namespace App.Services.Products;
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    private readonly IProductRepository _productRepository;

    public CreateProductRequestValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Name)
            //.NotNull().WithMessage("Ürün ismi gereklidir.")
            .NotEmpty().WithMessage("Ürün ismi gereklidir.")
            .Length(3, 10).WithMessage("Ürün ismi 3-10 karakter arasında olmalıdır.");
        //.MustAsync(MustUniqueProductNameAsync).WithMessage("ürün ismi veritabanında bulunmaktadır");
        // .Must(MustUniqueProductName).WithMessage("ürün ismi veritabanında bulunmaktadır");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");

        RuleFor(x => x.Stock)
            .InclusiveBetween(1, 100).WithMessage("stock adedi 1-100 arasında olmalıdır.");
    }

    #region 2.yol asenkron validation
    //private async Task<bool> MustUniqueProductNameAsync(string name, CancellationToken cancellationToken)
    //{
    //    return !await _productRepository.Where(x => x.Name == name).AnyAsync(cancellationToken);
    //}
    #endregion

    #region 1.yol senkron validation

    //private bool MustUniqueProductName(string name)
    //{
    //    return !_productRepository.Where(x => x.Name == name).Any();
    //}
    #endregion
}
