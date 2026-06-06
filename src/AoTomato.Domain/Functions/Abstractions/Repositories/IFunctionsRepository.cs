namespace AoTomato.Domain.Functions.Abstractions.Repositories;

using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.Functions.Enums;
using AoTomato.Domain.Functions.Models;

public interface IFunctionsRepository : IRepositoryBase<Function>
{
    Task<Function> GetByRouteAndMethodAsync(string routeKey, FunctionMethods method);
    Task<IEnumerable<Function>> GetWithCronAsync();
}