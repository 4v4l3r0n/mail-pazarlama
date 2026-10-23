using MailPazarlama.Data.Repositories;

namespace MailPazarlama.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
}
