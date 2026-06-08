namespace AoTomato.Domain.FunctionLogs.Abstractions.Services;

using AoTomato.domain.FunctionLogs.Dtos;
using AoTomato.Domain.Common.Models;
using AoTomato.Domain.FunctionLogs.Models;

public interface IFunctionLogsService : IServiceBase<FunctionLogDto, FunctionLog>
{
    Task<List<FunctionLogDto>> GetByFunctionAsync(string functionId);
    Task<List<FunctionLogDto>> GetByFunctionAndExecutionAsync(string functionId, string executionId);
    Task DeleteByFunctionAsync(string functionId);
    Task DeleteByFunctionAndExecutionAsync(string functionId, string executionId);
}