namespace AoTomato.Domain.Functions.Abstractions.Services;

using System.Text.Json;
using AoTomato.domain.Functions.Dtos;
using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Functions.Enums;
using AoTomato.Domain.Functions.Models;

public interface IFunctionsService : IServiceBase<FunctionDto, Function>
{
    Task<JsonDocument> ExecuteFunctionAsync(string routeKey, FunctionMethods method, JsonDocument? body, Dictionary<string, string> headers, Dictionary<string, string> queryParameters, string? apiKey = null);
    Task ExecuteFunctionCronAsync(string functionId);
    Task<IEnumerable<FunctionCron>> GetCronAsync();
}