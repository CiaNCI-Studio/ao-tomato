using AoTomato.Domain.Common.Models;

namespace AoTomato.Domain.Common.Abstractions.Repositories;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    Task DeleteAsync(string id, string deletedBy);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(string id);
    Task<TEntity> InsertAsync(TEntity entity, string createdBy);
    Task<TEntity> UpdateAsync(TEntity entity, string updatedBy);
}