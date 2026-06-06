using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.Variables.Models;

namespace AoTomato.Domain.Variables.Abstractions.Repositories;

public interface IVariablessRepository : IRepositoryBase<Variable>
{
    Task<Variable> GetByKeyAsync(string key);
}