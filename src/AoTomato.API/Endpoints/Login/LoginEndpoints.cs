using AoTomato.Domain.Login.Abstractions.Services;
using AoTomato.Domain.Login.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AoTomato.API.Endpoints.Login;

public static class LoginEndpoints
{
    public static WebApplication MapLoginEndpoints(this WebApplication app)
    {
        app.MapPost("v1/login", async ([FromServices] ILoginService service, [FromBody] LoginDto loginDto) =>
        {
            try
            {
                var loggedUser = await service.Login(loginDto);
                return Results.Ok(loggedUser);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .WithName("Login")
        .WithDescription("Authenticate user")
        .WithTags("Login")
        .AllowAnonymous()
        .Produces<LoggedUserDto>();

        app.MapGet("v1/login/initialCheck", async ([FromServices] ILoginService service) =>
        {
            var initialCheckResult = await service.InitialCheck();
            return Results.Ok(initialCheckResult);
        })
        .WithName("InitialCheck")
        .WithDescription("Verify Fresh Install")
        .WithTags("Login")
        .AllowAnonymous()
        .Produces<bool>();

        app.MapPost("v1/login/initialSetup", async ([FromServices] ILoginService service, [FromBody] InitialSetupDto initialSetupDto) =>
        {
            if(!await service.InitialCheck())
            {
                return Results.Unauthorized();
            }
            if (initialSetupDto == null || string.IsNullOrEmpty(initialSetupDto.UserName) || string.IsNullOrEmpty(initialSetupDto.UserEmail) || string.IsNullOrEmpty(initialSetupDto.UserPassword))
            {
                return Results.BadRequest("Invalid input data");
            }
            var initialCheckResult = await service.InitialSetup(initialSetupDto);
            return Results.Ok(initialCheckResult);
        })
        .WithName("InitialSetup")
        .WithDescription("Perform initial setup")
        .WithTags("Login")
        .AllowAnonymous()
        .Produces<bool>();

        return app;
    }
}

