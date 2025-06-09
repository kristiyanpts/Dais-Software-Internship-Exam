using FinalProject.Repository.Helpers;

namespace FinalProject.Repository.Base
{
    public interface IBaseRepository<T, TUpdate> where T : class
    {
        Task<int> Create(T entity);
        Task<T> Retrieve(int id);
        Task<T> Retrieve(string id);
        IAsyncEnumerable<T> RetrieveAll(QueryParameters queryParameters = null);
        Task<bool> Update(int id, TUpdate update);
        Task<bool> Update(string id, TUpdate update);
        Task<bool> Delete(int id);
        Task<bool> Delete(string id);
    }
}