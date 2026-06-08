using System.Net.Http.Headers;
using System.Text.Json;
using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Common.Dtos;
using AoTomato.Domain.Common.Enums;
using AoTomato.domain.FunctionLogs.Dtos;

namespace AoTomato.Admin.Services;

public class FunctionLogsService : IFunctionLogsService
{
    private readonly HttpClient httpClient;

    public FunctionLogsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<FunctionLogDto>> GetByFunctionAsync(string functionId, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.GetAsync($"/v1/functionlogs/by-function/{functionId}");
        return await ParseListResponseAsync(response);
    }

    public async Task<List<FunctionLogDto>> GetByFunctionAndExecutionAsync(string functionId, string executionId, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.GetAsync($"/v1/functionlogs/by-function/{functionId}/{executionId}");
        return await ParseListResponseAsync(response);
    }

    public async Task<bool> DeleteByFunctionAsync(string functionId, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.DeleteAsync($"/v1/functionlogs/by-function/{functionId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteByFunctionAndExecutionAsync(string functionId, string executionId, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.DeleteAsync($"/v1/functionlogs/by-function/{functionId}/{executionId}");
        return response.IsSuccessStatusCode;
    }

    private static async Task<List<FunctionLogDto>> ParseListResponseAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<List<FunctionLogDto>>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
                return result.Payload ?? [];
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        return [];
    }
}
