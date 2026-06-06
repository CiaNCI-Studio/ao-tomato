using AoTomato.Domain.Login.Dtos;

namespace AoTomato.Domain.Common.Models;

public interface IServiceBase<TDto, TEntity> 
    where TDto : class
    where TEntity : EntityBase, new()
{
    public Task<TDto> CreateAsync(TDto dto, LoggedUserDto loggedUser);

    public Task<TDto> UpdateAsync(TDto dto, LoggedUserDto loggedUser);

    public Task DeleteAsync(string id, LoggedUserDto loggedUser);

    public Task<TDto> GetByIdAsync(string id, LoggedUserDto loggedUser);

    public Task<List<TDto>> GetAllAsync(LoggedUserDto loggedUser);
}