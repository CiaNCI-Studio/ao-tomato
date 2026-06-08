using AoTomato.domain.FunctionLogs.Dtos;

namespace AoTomato.Admin.Abstractions;

public interface IFunctionLogsService
{
    Task<List<FunctionLogDto>> GetByFunctionAsync(string functionId, string token);
    Task<List<FunctionLogDto>> GetByFunctionAndExecutionAsync(string functionId, string executionId, string token);
    Task<bool> DeleteByFunctionAsync(string functionId, string token);
    Task<bool> DeleteByFunctionAndExecutionAsync(string functionId, string executionId, string token);
}
