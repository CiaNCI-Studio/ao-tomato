using AoTomato.API.Extensions;
using AoTomato.Domain.Functions.Abstractions.Services;
using AoTomato.domain.Functions.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AoTomato.API.Endpoints.Functions;

public static class FunctionsEndpoints
{
    public static WebApplication MapFunctionsEndpoints(this WebApplication app)
    {
        app.MapGet("v1/functions", async (HttpContext context, [FromServices] IFunctionsService service) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var functions = await service.GetAllAsync(loggedUser);
            return Results.Ok(functions);   
        })
        .WithName("GetFunctions")
        .WithDescription("Get functions")
        .WithTags("Functions")
        .Produces<List<FunctionDto>>();  

        app.MapGet("v1/function/{id}", async (HttpContext context, [FromServices] IFunctionsService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.GetByIdAsync(id, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(target);
        })
        .WithName("GetFunction")
        .WithDescription("Get function by id")
        .WithTags("Functions")
        .Produces<FunctionDto>();  

        app.MapPut("v1/function", async (HttpContext context, [FromServices] IFunctionsService service, [FromBody] FunctionDto dto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.UpdateAsync(dto, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(target);
        })
        .WithName("UpdateFunction")
        .WithDescription("Update function")
        .WithTags("Functions")
        .Produces<FunctionDto>();  

        app.MapPost("v1/function", async (HttpContext context, [FromServices] IFunctionsService service, [FromBody] FunctionDto dto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.CreateAsync(dto, loggedUser);
            if (target == null) return Results.BadRequest();
            return Results.Ok(target);
        })
        .WithName("CreateFunction")
        .WithDescription("Create function")
        .WithTags("Functions")
        .Produces<FunctionDto>();  

        app.MapDelete("v1/function/{id}", async (HttpContext context, [FromServices] IFunctionsService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            await service.DeleteAsync(id, loggedUser);
            return Results.Ok();
        })
        .WithName("DeleteFunction")
        .WithDescription("Delete function")
        .WithTags("Functions");  

        return app;
    }
}
