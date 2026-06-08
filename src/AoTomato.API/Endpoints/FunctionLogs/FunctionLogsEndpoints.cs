using AoTomato.API.Extensions;
using AoTomato.Domain.FunctionLogs.Abstractions.Services;
using AoTomato.domain.FunctionLogs.Dtos;
using Microsoft.AspNetCore.Mvc;
using AoTomato.Domain.Common.Dtos;

namespace AoTomato.API.Endpoints.FunctionLogs;

public static class FunctionLogsEndpoints
{
    public static WebApplication MapFunctionLogsEndpoints(this WebApplication app)
    {
        app.MapGet("v1/functionlogs", async (HttpContext context, [FromServices] IFunctionLogsService service) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var logs = await service.GetAllAsync(loggedUser);
            return Results.Ok(Response<List<FunctionLogDto>>.Ok(logs));
        })
        .WithName("GetFunctionLogs")
        .WithDescription("Get all function logs")
        .WithTags("FunctionLogs")
        .Produces<Response<List<FunctionLogDto>>>();

        app.MapGet("v1/functionlog/{id}", async (HttpContext context, [FromServices] IFunctionLogsService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.GetByIdAsync(id, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(Response<FunctionLogDto>.Ok(target));
        })
        .WithName("GetFunctionLog")
        .WithDescription("Get function log by id")
        .WithTags("FunctionLogs")
        .Produces<Response<FunctionLogDto>>();

        app.MapGet("v1/functionlogs/by-function/{functionId}", async (HttpContext context, [FromServices] IFunctionLogsService service, string functionId) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var logs = await service.GetByFunctionAsync(functionId);
            return Results.Ok(Response<List<FunctionLogDto>>.Ok(logs));
        })
        .WithName("GetFunctionLogsByFunction")
        .WithDescription("Get function logs by function id")
        .WithTags("FunctionLogs")
        .Produces<Response<List<FunctionLogDto>>>();

        app.MapGet("v1/functionlogs/by-function/{functionId}/{executionId}", async (HttpContext context, [FromServices] IFunctionLogsService service, string functionId, string executionId) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var logs = await service.GetByFunctionAndExecutionAsync(functionId, executionId);
            return Results.Ok(Response<List<FunctionLogDto>>.Ok(logs));
        })
        .WithName("GetFunctionLogsByFunctionAndExecution")
        .WithDescription("Get function logs by function id and Execution Id")
        .WithTags("FunctionLogs")
        .Produces<Response<List<FunctionLogDto>>>();

        app.MapDelete("v1/functionlogs/by-function/{functionId}", async (HttpContext context, [FromServices] IFunctionLogsService service, string functionId) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            await service.DeleteByFunctionAsync(functionId);
            return Results.Ok();
        })
        .WithName("DeleteFunctionLogsByFunction")
        .WithDescription("Delete all function logs by function id")
        .WithTags("FunctionLogs")
        .Produces<Response<bool>>();

        app.MapDelete("v1/functionlogs/by-function/{functionId}/{executionId}", async (HttpContext context, [FromServices] IFunctionLogsService service, string functionId, string executionId) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            await service.DeleteByFunctionAndExecutionAsync(functionId, executionId);
            return Results.Ok();
        })
        .WithName("DeleteFunctionLogsByFunctionAndExecution")
        .WithDescription("Delete all function logs by function id and Execution Id")
        .WithTags("FunctionLogs")
        .Produces<Response<bool>>();

        return app;
    }
}
