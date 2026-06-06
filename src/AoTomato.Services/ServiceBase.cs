using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Login.Dtos;
using AutoMapper;
using Serilog;

namespace AoTomato.Services;

public class ServiceBase<TDto, TEntity> : IServiceBase<TDto, TEntity>
    where TEntity : EntityBase, new()
    where TDto : class
{
    protected readonly IMapper mapper;
    protected readonly IRepositoryBase<TEntity> repository;
    protected readonly ILogger logger;

    public ServiceBase(IRepositoryBase<TEntity> repository, ILogger logger, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<TDto> CreateAsync(TDto dto, LoggedUserDto loggedUser)
    {
        var entity = mapper.Map<TEntity>(dto);
        entity = await repository.InsertAsync(entity, loggedUser.User.Id);
        return mapper.Map<TDto>(entity);
    }

    public async Task DeleteAsync(string id, LoggedUserDto loggedUser)
    {
        await repository.DeleteAsync(id, loggedUser.User.Id);
    }

    public async Task<List<TDto>> GetAllAsync(LoggedUserDto loggedUser)
    {
        var entities = await repository.GetAllAsync();
        return mapper.Map<List<TDto>>(entities);
    }

    public async Task<TDto> GetByIdAsync(string id, LoggedUserDto loggedUser)
    {
        var entity = await repository.GetByIdAsync(id);
        return mapper.Map<TDto>(entity);
    }

    public async Task<TDto> UpdateAsync(TDto dto, LoggedUserDto loggedUser)
    {
        var entity = mapper.Map<TEntity>(dto);
        entity = await repository.UpdateAsync(entity, loggedUser.User.Id);
        return mapper.Map<TDto>(entity);
    }
}
