namespace AoTomato.Services.Functions;

using NLua;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using AoTomato.Domain.Functions.Abstractions.Repositories;
using AoTomato.Domain.Functions.Abstractions.Services;
using AoTomato.domain.Functions.Dtos;
using AoTomato.Domain.Functions.Models;
using AutoMapper;
using AoTomato.Domain.Functions.Enums;
using AoTomato.Domain.Exceptions;
using AoTomato.Services.Helpers;
using AoTomato.Domain.Variables.Abstractions.Services;
using AoTomato.Domain.Variables.Dtos;
using Serilog;
using AoTomato.Domain.FunctionLogs.Abstractions.Services;
using AoTomato.Domain.FunctionLogs.Models;
using AoTomato.domain.FunctionLogs.Dtos;
using AoTomato.Domain.Helpers;

public class FunctionsService : ServiceBase<FunctionDto, Function>, IFunctionsService
{
    private readonly IFunctionsRepository functionsRepository;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IVariablesService variablesService;
    private readonly IFunctionLogsService functionLogsService;

    private string currentFunctionId = string.Empty;
    private string currentExecutionId = string.Empty;

    public FunctionsService(IFunctionsRepository functionsRepository,
                            IVariablesService variablesService,
                            IFunctionLogsService functionLogsService,
                            ILogger logger,
                            IMapper mapper,
                            IHttpClientFactory httpClientFactory) : base(functionsRepository, logger, mapper)
    {
        this.functionsRepository = functionsRepository;
        this.httpClientFactory = httpClientFactory;
        this.variablesService = variablesService;
        this.functionLogsService = functionLogsService;
    }

    public async Task<IEnumerable<FunctionCron>> GetCronAsync()
    {
        return await functionsRepository.GetCronAsync(); 
    }

    public async Task ExecuteFunctionCronAsync(string functionId)
    {
        var function = await functionsRepository.GetByIdAsync(functionId);
        
        if (function == null)
            throw new NotFoundException("Function not found");

        await StartFunctionExecutionLog("Function execution Started by cron.", function, null, new Dictionary<string, string>(), new Dictionary<string, string>());
        await ExecuteFunctionAsync(function);
    }

    public async Task<JsonDocument> ExecuteFunctionAsync(string routeKey,
                                                   FunctionMethods method,
                                                   JsonDocument? body,
                                                   Dictionary<string, string> headers,
                                                   Dictionary<string, string> queryParameters,
                                                   string? apiKey = null)
    {
        var function = await functionsRepository.GetByRouteAndMethodAsync(routeKey, method);
        
        if (function == null)
            throw new NotFoundException("Function not found");

        if (!string.IsNullOrEmpty(function.ApiKey) && function.ApiKey != apiKey)
            throw new UnauthorizedAccessException("Invalid API key");

        await StartFunctionExecutionLog("Function execution Started by request.", function, body, headers, queryParameters);
        return await ExecuteFunctionAsync(function, body, headers, queryParameters);

    }

    private async Task<JsonDocument> ExecuteFunctionAsync(Function function)
    {
        return await ExecuteFunctionAsync(function, null, new Dictionary<string, string>(), new Dictionary<string, string>());
    }

    private async Task<JsonDocument> ExecuteFunctionAsync(Function function, JsonDocument? body, Dictionary<string, string> headers, Dictionary<string, string> queryParameters)
    {
        using (var lua = new Lua())
        {
            lua.State.Encoding = Encoding.UTF8;
            lua.State.OpenLibs();

            lua.NewTable("response");
            lua["response.status"] = 200L;
            lua["response.body"] = "";
            lua["response.headers"] = LuaHelpers.NewLuaTable(lua);

            lua.NewTable("ctx");
            lua["ctx.method"] = function.Method.ToString().ToUpper();

            InjectHeaders(lua, headers);
            InjectQueryParameters(lua, queryParameters);
            InjectBody(lua, body);

            LuaHelpers.LoadJsonLibrary(lua);
            RegisterHttpLibrary(lua);
            RegisterVariablesLibrary(lua);
            RegisterLogLibrary(lua);

            try
            {
                lua.DoString(function.Code);
                var responseTable = (LuaTable)lua["response"];
                var responseJson = LuaHelpers.LuaTableToJsonDocument(responseTable);
                await FinishFunctionExecutionLog(function, body, headers, queryParameters, responseJson.ToJsonString() ?? string.Empty);
                return responseJson;
            }
            catch (NLua.Exceptions.LuaScriptException ex)
            {
                await ErrorFunctionExecutionLog(function, body, headers, queryParameters, ex.Message);
                throw new ApplicationException($"Lua execution error: {ex.Message}", ex);
            }
        }
    }

    private static void InjectHeaders(Lua lua, Dictionary<string, string> headers)
    {
        lua.NewTable("headers");
        var table = (LuaTable)lua["headers"];
        foreach (var kvp in headers)
            table[kvp.Key] = kvp.Value;
    }

    private static void InjectQueryParameters(Lua lua, Dictionary<string, string> queryParameters)
    {
        lua.NewTable("queryParameters");
        var table = (LuaTable)lua["queryParameters"];
        foreach (var kvp in queryParameters)
            table[kvp.Key] = kvp.Value;
    }

    private static void InjectBody(Lua lua, JsonDocument? body)
    {
        if (body != null)
            lua["body"] = LuaHelpers.JsonElementToLuaTable(lua, body.RootElement);
        else
            lua["body"] = null;
    }

    private void RegisterHttpLibrary(Lua lua)
    {
        lua.NewTable("http");

        var httpTable = (LuaTable)lua["http"];
        var factory = httpClientFactory;

        httpTable["request"] = new Func<LuaTable, LuaTable>(options =>
        {
            var methodStr = options["method"]?.ToString() ?? "GET";
            var url = options["url"]?.ToString() ?? "";

            using var client = factory.CreateClient();
            using var request = new HttpRequestMessage(new HttpMethod(methodStr), url);

            if (options["headers"] is LuaTable reqHeaders)
            {
                foreach (KeyValuePair<object, object> kvp in reqHeaders)
                    request.Headers.TryAddWithoutValidation(kvp.Key.ToString()!, kvp.Value?.ToString());
            }

            if (options["body"] is string reqBody && !string.IsNullOrEmpty(reqBody))
            {
                var contentType = options["headers"] is LuaTable h
                    && h["Content-Type"]?.ToString() is string ct ? ct : MediaTypeNames.Application.Json;
                request.Content = new StringContent(reqBody, Encoding.UTF8, contentType);
            }

            HttpResponseMessage response;
            try
            {
                response = Task.Run(async () => await client.SendAsync(request)).Result;
            }
            catch
            {
                return LuaHelpers.BuildLuaTableFromJson(lua, "{\"status\":0,\"body\":\"\",\"headers\":{}}");
            }

            var resultJson = $$"""{"status":{{(int)response.StatusCode}},"body":{{JsonSerializer.Serialize(Task.Run(async () => await response.Content.ReadAsStringAsync()).Result)}},"headers":{{SerializeHeadersToJson(response)}}}""";
            return LuaHelpers.BuildLuaTableFromJson(lua, resultJson);
        });
    }

     private void RegisterVariablesLibrary(Lua lua)
    {
        lua.NewTable("variables");

        var variablesTable = (LuaTable)lua["variables"];
        var factory = httpClientFactory;

        variablesTable["get"] = new Func<string, string?>(variableKey =>
        {
            var variable = variablesService.GetByKeyAsync(variableKey).Result;
            if(variable != null)
            {
                return variable.Value;
            }
            return null;
        });

         variablesTable["set"] = new Func<LuaTable, bool>(variableTable =>
        {
            if (string.IsNullOrEmpty(variableTable["key"]?.ToString()))
            {
                logger.Warning("Missing variable key on set.");
                return false;
            }
            var variable = new VariableDto
            {
                Key = variablesTable["key"]?.ToString() ?? string.Empty,
                Value = variablesTable["value"]?.ToString() ?? string.Empty
            };
            return variablesService.SetVariableAsync(variable).Result;
        });
    }

     private void RegisterLogLibrary(Lua lua)
    {
        lua.NewTable("log");

        var logTable = (LuaTable)lua["log"];
        var factory = httpClientFactory;

        logTable["write"] = new Action<LuaTable>(async logInfo =>
        {
            _ = CreateFunctionExecutionLog(logInfo["message"]?.ToString() ?? string.Empty, logInfo["body"]?.ToString() ?? string.Empty);
        });
    }

    private static string SerializeHeadersToJson(HttpResponseMessage response)
    {
        var allHeaders = new Dictionary<string, string>();
        foreach (var h in response.Headers)
            allHeaders[h.Key] = string.Join(", ", h.Value);
        foreach (var h in response.Content.Headers)
            allHeaders[h.Key] = string.Join(", ", h.Value);
        return JsonSerializer.Serialize(allHeaders);
    }

    private async Task StartFunctionExecutionLog(string message, 
                                                    Function function, 
                                                    JsonDocument? body,
                                                    Dictionary<string, string> headers,
                                                    Dictionary<string, string> queryParameters)
    {
        currentExecutionId = Guid.NewGuid().ToString();
        currentFunctionId = function.Id;
        var functionLog = new FunctionLogDto
        {
          FunctionId = currentFunctionId,
          ExecutionId = currentExecutionId,
          Message = message,
          Body = body?.ToJsonString() ?? string.Empty,
          Headers = "[" + string.Join(", ", headers.Select((item) => $"{{ 'Key' : {item.Key} : {item.Value} }}")) + "]",
          QueryParameters = "[" + string.Join(", ", queryParameters.Select((item) => $"{{ 'Key' : {item.Key} : {item.Value} }}")) + "]"
        };
        await functionLogsService.CreateAsync(functionLog, null);
    }

    private async Task FinishFunctionExecutionLog(Function function, 
                                                    JsonDocument? body,
                                                    Dictionary<string, string> headers,
                                                    Dictionary<string, string> queryParameters,
                                                    string result)
    {
        var functionLog = new FunctionLogDto
        {
          FunctionId = currentFunctionId,
          ExecutionId = currentExecutionId,
          Message = "Function execution Finished",
          Body = body?.ToString() ?? string.Empty,
          Headers = "[" + string.Join(", ", headers.Select((item) => $"{{ 'Key' : {item.Key} : {item.Value} }}")) + "]",
          QueryParameters = "[" + string.Join(", ", queryParameters.Select((item) => $"{{ 'Key' : {item.Key} : {item.Value} }}")) + "]",
          Result = result
        };
        await functionLogsService.CreateAsync(functionLog, null);
        currentExecutionId = string.Empty;
        currentFunctionId = string.Empty;
    }

     private async Task ErrorFunctionExecutionLog(Function function, 
                                                    JsonDocument? body,
                                                    Dictionary<string, string> headers,
                                                    Dictionary<string, string> queryParameters,
                                                    string error)
    {
        var functionLog = new FunctionLogDto
        {
          FunctionId = currentFunctionId,
          ExecutionId = currentExecutionId,
          Message = "ERROR: " + error,
          Body = body?.ToString() ?? string.Empty,
          Headers = "[" + string.Join(", ", headers.Select((item) => $"{{ 'Key' : {item.Key} : {item.Value} }}")) + "]",
          QueryParameters = "[" + string.Join(", ", queryParameters.Select((item) => $"{{ 'Key' : {item.Key} : {item.Value} }}")) + "]"
        };
        await functionLogsService.CreateAsync(functionLog, null);
        currentFunctionId = string.Empty;
        currentExecutionId = string.Empty;
    }

    private async Task CreateFunctionExecutionLog(string message, string body)
    {
        var functionLog = new FunctionLogDto
        {
          FunctionId = currentFunctionId,
          ExecutionId = currentExecutionId,
          Message = message,
          Body = body
        };
        await functionLogsService.CreateAsync(functionLog, null);
    }

    
}
