namespace AoTomato.Admin.Abstractions;


public interface IServiceBase<T>
{
    Task<List<T>> GetAll(string token);
    
    Task<T?> GetById(string id, string token);

    Task<T?> Save(T dto, bool create, string token);

    Task<bool> Delete(string id, string token);
}