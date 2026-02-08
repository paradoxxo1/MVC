
using App.Application.Contracts.Persistence;

namespace App.Repositories;
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangeAsync() => context.SaveChangesAsync();
}
