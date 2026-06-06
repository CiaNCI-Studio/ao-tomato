using AoTomato.API.Extensions;
using AoTomato.Domain.Functions.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AoTomato.Domain.Functions.Enums;

namespace AoTomato.API.Endpoints.Functions;

public static class AoEndpoints
{
    public static WebApplication MapAoEndpoints(this WebApplication app)
    {
        app.MapGet("ao/{routeKey}", async (HttpContext context, [FromServices] IFunctionsService service, string routeKey) =>
        {
            var apiKey = context.GetApiKey();
            var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var queryParams = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            var functionResult = await service.ExecuteFunctionAsync(routeKey, FunctionMethods.Get, null, headers, queryParams, apiKey);
            return Results.Ok(functionResult);   
        })
        .WithName("AoGet")
        .WithDescription("Get Ao function")
        .WithTags("AoTomato")
        .Produces<JsonDocument>();  

        app.MapPost("ao/{routeKey}", async (HttpContext context, [FromServices] IFunctionsService service, [FromBody] JsonDocument body, string routeKey) =>
        {
            var apiKey = context.GetApiKey();
            var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var queryParams = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            var functionResult = await service.ExecuteFunctionAsync(routeKey, FunctionMethods.Post, body, headers, queryParams, apiKey);
            return Results.Ok(functionResult);   
        })
        .WithName("AoPost")
        .WithDescription("Post Ao function")
        .WithTags("AoTomato")
        .Produces<JsonDocument>();  

        app.MapPut("ao/{routeKey}", async (HttpContext context, [FromServices] IFunctionsService service, [FromBody] JsonDocument body, string routeKey) =>
        {
            var apiKey = context.GetApiKey();
            var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var queryParams = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            var functionResult = await service.ExecuteFunctionAsync(routeKey, FunctionMethods.Put, body, headers, queryParams, apiKey);
            return Results.Ok(functionResult);   
        })
        .WithName("AoPut")
        .WithDescription("Put Ao function")
        .WithTags("AoTomato")
        .Produces<JsonDocument>();  

        app.MapPatch("ao/{routeKey}", async (HttpContext context, [FromServices] IFunctionsService service, [FromBody] JsonDocument body, string routeKey) =>
        {
            var apiKey = context.GetApiKey();
            var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var queryParams = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            var functionResult = await service.ExecuteFunctionAsync(routeKey, FunctionMethods.Patch, body, headers, queryParams, apiKey);
            return Results.Ok(functionResult);   
        })
        .WithName("AoPatch")
        .WithDescription("Patch Ao function")
        .WithTags("AoTomato")
        .Produces<JsonDocument>();  

        app.MapDelete("ao/{routeKey}", async (HttpContext context, [FromServices] IFunctionsService service, string routeKey) =>
        {
            var apiKey = context.GetApiKey();
            var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var queryParams = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            var functionResult = await service.ExecuteFunctionAsync(routeKey, FunctionMethods.Delete, null, headers, queryParams, apiKey);
            return Results.Ok(functionResult);   
        })
        .WithName("AoDelete")
        .WithDescription("Delete Ao function")
        .WithTags("AoTomato")
        .Produces<JsonDocument>();  

        return app;
    }
}
