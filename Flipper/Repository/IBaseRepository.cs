namespace Flipper.Repository;

public interface IBaseRepository<T> where T:class
{
    public Task Add(T item);
    public Task AddRange(List<T> item);
    public Task Update(T item);
    public  Task UpdateRange(List<T> item);

    public Task<List<T>> GetRange();
    public Task<T> Get(string name, string detailsId);
}