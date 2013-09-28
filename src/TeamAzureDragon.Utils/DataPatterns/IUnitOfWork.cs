namespace WorkingWithDataMvc.Data
{
    using System;

    using WorkingWithDataMvc.Models;

    public interface IUowData : IDisposable
    {
        IRepository<T> GetRepo<T>();

        int SaveChanges();
    }
}
