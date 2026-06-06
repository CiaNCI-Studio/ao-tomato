using AoTomato.API.Extensions;
using AoTomato.Domain.Variables.Abstractions.Services;
using AoTomato.Domain.Variables.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AoTomato.API.Endpoints.Variables;

public static class VariablesEndpoints
{
    public static WebApplication MapVariablesEndpoints(this WebApplication app)
    {
        app.MapGet("v1/variables", async (HttpContext context, [FromServices] IVariablesService service) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var variables = await service.GetAllAsync(loggedUser);
            return Results.Ok(variables);   
        })
        .WithName("GetVariables")
        .WithDescription("Get variables")
        .WithTags("Variables")
        .Produces<List<VariableDto>>();  

        app.MapGet("v1/variable/{id}", async (HttpContext context, [FromServices] IVariablesService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.GetByIdAsync(id, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(target);
        })
        .WithName("GetVariable")
        .WithDescription("Get variable by id")
        .WithTags("Variables")
        .Produces<VariableDto>();  

        app.MapPut("v1/variable", async (HttpContext context, [FromServices] IVariablesService service, [FromBody] VariableDto dto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.UpdateAsync(dto, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(target);
        })
        .WithName("UpdateVariable")
        .WithDescription("Update variable")
        .WithTags("Variables")
        .Produces<VariableDto>();  

        app.MapPost("v1/variable", async (HttpContext context, [FromServices] IVariablesService service, [FromBody] VariableDto dto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.CreateAsync(dto, loggedUser);
            if (target == null) return Results.BadRequest();
            return Results.Ok(target);
        })
        .WithName("CreateVariable")
        .WithDescription("Create variable")
        .WithTags("Variables")
        .Produces<VariableDto>();  

        app.MapDelete("v1/variable/{id}", async (HttpContext context, [FromServices] IVariablesService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            await service.DeleteAsync(id, loggedUser);
            return Results.Ok();
        })
        .WithName("DeleteVariable")
        .WithDescription("Delete variable")
        .WithTags("Variables");  

        return app;
    }
}
