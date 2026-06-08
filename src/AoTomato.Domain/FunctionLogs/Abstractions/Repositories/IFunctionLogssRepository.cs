namespace AoTomato.Domain.FunctionLogs.Abstractions.Repositories;

using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.FunctionLogs.Models;

public interface IFunctionLogssRepository : IRepositoryBase<FunctionLog>
{
    Task<List<FunctionLog>> GetByFunctionAsync(string functionId);
    Task<List<FunctionLog>> GetByFunctionAndExecutionAsync(string functionId, string executionId);
    Task DeleteByFunctionAsync(string functionId);
    Task DeleteByFunctionAndExecutionAsync(string functionId, string executionId);
}