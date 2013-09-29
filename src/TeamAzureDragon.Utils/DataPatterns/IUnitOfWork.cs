
namespace TeamAzureDragon.Utils
{
    using System;

    public interface IUnitOfWork : IDisposable 
    {
        IRepository<T> _<T>() where T : class;

        int SaveChanges();
    }
}
