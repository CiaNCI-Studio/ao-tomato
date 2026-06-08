namespace AoTomato.Repositories.FunctionLogs;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.FunctionLogs.Abstractions.Repositories;
using AoTomato.Domain.FunctionLogs.Models;

public class FunctionLogsRepository : RepositoryBase<FunctionLog>, IFunctionLogssRepository
{
    public FunctionLogsRepository(DbSettings settings) : base(settings, "functionlogs")
    {
    }

    public async Task<List<FunctionLog>> GetByFunctionAsync(string functionId)
    {
        return await Task.Run(() => collection.Find(item => item.FunctionId == functionId).ToList());
    }

    public async Task DeleteByFunctionAsync(string functionId)
    {
        await Task.Run(() => collection.DeleteMany(item => item.FunctionId == functionId));
    }

    public async Task<List<FunctionLog>> GetByFunctionAndExecutionAsync(string functionId, string executionId)
    {
        return await Task.Run(() => collection.Find(item => item.FunctionId == functionId && item.ExecutionId == executionId).ToList());
    }

    public async Task DeleteByFunctionAndExecutionAsync(string functionId, string executionId)
    {
        await Task.Run(() => collection.DeleteMany(item => item.FunctionId == functionId && item.ExecutionId == executionId));
    }
}
