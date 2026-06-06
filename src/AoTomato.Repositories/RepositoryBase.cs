using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.Common.Enums;
using AoTomato.Domain.Common.Models;
using LiteDB;

namespace AoTomato.Repositories;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    protected readonly LiteDatabase database;
    protected readonly ILiteCollection<TEntity> collection;


    public RepositoryBase(DbSettings settings, string collectionName)
    {
        database = new LiteDatabase(settings.ConnectionString);
        collection = database.GetCollection<TEntity>(collectionName);
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity, string createdBy)
    {
        if (entity is EntityBase entityBase && string.IsNullOrEmpty(entityBase.Id))
        {
            entityBase.Id = Guid.NewGuid().ToString();
        }
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.CreatedBy = createdBy;
        entity.UpdatedBy = createdBy;
        await Task.Run(() => collection.Insert(entity));
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, string updatedBy)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        await Task.Run(() => collection.Update(entity));
        return entity;
    }

    public virtual async Task DeleteAsync(string id, string deletedBy)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deletedBy;
            entity.State = EntityStates.Deleted;
            await Task.Run(() => collection.Update(entity));
        }
    }

    public virtual async Task<TEntity> GetByIdAsync(string id)
    {
        return await Task.Run(() => collection.FindById(id));
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {

        return await Task.Run(() => collection.Find(item => item.State == EntityStates.Active));
    }
}
