namespace AoTomato.Repositories.Variables;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Variables.Abstractions.Repositories;
using AoTomato.Domain.Variables.Models;

public class VariableRepository : RepositoryBase<Variable>, IVariablessRepository
{
    public VariableRepository(DbSettings settings) : base(settings, "variables")
    {
    }

    public async Task<Variable> GetByKeyAsync(string key)
    {
        return await Task.Run(() => collection.FindOne(item => item.Key == key));
    }
}
