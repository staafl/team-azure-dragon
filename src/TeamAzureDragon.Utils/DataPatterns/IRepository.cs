namespace TeamAzureDragon.Utils
{
    using System.Linq;

    public interface IRepository<T>  where T : class
    {
        IQueryable<T> All();

        IQueryable<T> All(params string[] include);

        T GetById(int id);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(int id);

        void Detach(T entity);
    }
}
