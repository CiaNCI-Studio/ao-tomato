namespace AoTomato.Repositories.Functions;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Functions.Abstractions.Repositories;
using AoTomato.Domain.Functions.Enums;
using AoTomato.Domain.Functions.Models;

public class FunctionsRepository : RepositoryBase<Function>, IFunctionsRepository
{
    public FunctionsRepository(DbSettings settings) : base(settings, "functions")
    {
    }

    public Task<Function> GetByRouteAndMethodAsync(string routeKey, FunctionMethods method)
    {
        return Task.Run(() =>
        {
            return collection.Find(item => item.Route == routeKey && item.Method == method).FirstOrDefault();
        });
    }

    public async Task<IEnumerable<Function>> GetWithCronAsync()
    {
        return await Task.Run(() => collection.Find(item => !string.IsNullOrEmpty(item.Cron)));
    }
}
