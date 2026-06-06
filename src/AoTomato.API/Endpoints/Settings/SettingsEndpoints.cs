using AoTomato.API.Extensions;
using AoTomato.Domain.Settings.Abstractions.Services;
using AoTomato.Domain.Settings.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AoTomato.API.Endpoints.Settings;

public static class SettingsEndpoints
{
    public static WebApplication MapSettingsEndpoints(this WebApplication app)
    {
        app.MapGet("v1/settings", async (HttpContext context, [FromServices] ISettingsService service) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var settings = await service.GetAllAsync(loggedUser);
            return Results.Ok(settings);   
        })
        .WithName("GetSettings")
        .WithDescription("Get settings")
        .WithTags("Settings")
        .Produces<List<SettingDto>>();  

        app.MapGet("v1/setting/{id}", async (HttpContext context, [FromServices] ISettingsService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.GetByIdAsync(id, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(target);
        })
        .WithName("GetSetting")
        .WithDescription("Get setting by id")
        .WithTags("Settings")
        .Produces<SettingDto>();  

        app.MapPut("v1/setting", async (HttpContext context, [FromServices] ISettingsService service, [FromBody] SettingDto dto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.UpdateAsync(dto, loggedUser);
            if (target == null) return Results.NotFound();
            return Results.Ok(target);
        })
        .WithName("UpdateSetting")
        .WithDescription("Update setting")
        .WithTags("Settings")
        .Produces<SettingDto>();  

        app.MapPost("v1/setting", async (HttpContext context, [FromServices] ISettingsService service, [FromBody] SettingDto dto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var target = await service.CreateAsync(dto, loggedUser);
            if (target == null) return Results.BadRequest();
            return Results.Ok(target);
        })
        .WithName("CreateSetting")
        .WithDescription("Create setting")
        .WithTags("Settings")
        .Produces<SettingDto>();  

        app.MapDelete("v1/setting/{id}", async (HttpContext context, [FromServices] ISettingsService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            await service.DeleteAsync(id, loggedUser);
            return Results.Ok();
        })
        .WithName("DeleteSetting")
        .WithDescription("Delete setting")
        .WithTags("Settings");  

        return app;
    }
}
