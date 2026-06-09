namespace AoTomato.Services.FunctionLogs;

using AoTomato.domain.FunctionLogs.Dtos;
using AoTomato.Domain.FunctionLogs.Abstractions.Repositories;
using AoTomato.Domain.FunctionLogs.Abstractions.Services;
using AoTomato.Domain.FunctionLogs.Models;
using AutoMapper;
using Serilog;

public class FunctionLogsService : ServiceBase<FunctionLogDto, FunctionLog>, IFunctionLogsService
{
    private readonly IFunctionLogssRepository functionLogsRepository;

    public FunctionLogsService(IFunctionLogssRepository functionLogsRepository, ILogger logger, IMapper mapper)
        : base(functionLogsRepository, logger, mapper)
    {
        this.functionLogsRepository = functionLogsRepository;
    }

    public async Task<List<FunctionLogDto>> GetByFunctionAsync(string functionId)
    {
        var entities = await functionLogsRepository.GetByFunctionAsync(functionId);
        return mapper.Map<List<FunctionLogDto>>(entities);
    }

    public async Task DeleteByFunctionAsync(string functionId)
    {
        await functionLogsRepository.DeleteByFunctionAsync(functionId);
    }

    public async Task<List<FunctionLogDto>> GetByFunctionAndExecutionAsync(string functionId, string executionId)
    {
        var entities = await functionLogsRepository.GetByFunctionAndExecutionAsync(functionId, executionId);
        return mapper.Map<List<FunctionLogDto>>(entities);
    }

    public async Task DeleteByFunctionAndExecutionAsync(string functionId, string executionId)
    {
         await functionLogsRepository.DeleteByFunctionAndExecutionAsync(functionId, executionId);
    }

    public async Task DeleteByDateAsync(DateTime beforeDate)
    {
        await functionLogsRepository.DeleteByDateAsync(beforeDate);
    }
}
