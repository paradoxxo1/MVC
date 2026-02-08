using App.Domain.Entities.Common;
using App.Repositories;

namespace App.Application.Contracts.Persistence;
public interface IProductRepository : IGenericRepository<Product, int>
{
    public Task<List<Product>> GetTopPriceProductAsync(int count);
}
