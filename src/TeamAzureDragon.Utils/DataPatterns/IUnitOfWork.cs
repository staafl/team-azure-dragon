
namespace TeamAzureDragon.Utils
{
    using System;
    using System.Data.Entity;

    public interface IUnitOfWork : IDisposable 
    {
        DbContext Context { get; }
        IRepository<T> _<T>() where T : class;

        int SaveChanges();
    }
}
