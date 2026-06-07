using AoTomato.API.Extensions;
using AoTomato.Domain.Common.Dtos;
using AoTomato.Domain.Users.Abstractions.Services;
using AoTomato.Domain.Users.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AoTomato.API.Endpoints.Users;

public static class UsersEndpoints
{
    public static WebApplication MapUsersEndpoints(this WebApplication app)
    {
        app.MapGet("v1/users", async (HttpContext context, [FromServices] IUsersService service) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var users = await service.GetAllAsync(loggedUser);
            return Results.Ok(Response<List<UserDto>>.Ok(users));   
        })
        .WithName("GetUsers")
        .WithDescription("Get users")
        .WithTags("Users")
        .Produces<Response<List<UserDto>>>();  

        app.MapGet("v1/user/{id}", async (HttpContext context, [FromServices] IUsersService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var targetUser = await service.GetByIdAsync(id, loggedUser);
            if (targetUser == null) return Results.NotFound();
            return Results.Ok(Response<UserDto>.Ok(targetUser));
        })
        .WithName("GetUser")
        .WithDescription("Get user by id")
        .WithTags("Users")
        .Produces<Response<UserDto>>();  

        app.MapPut("v1/user", async (HttpContext context, [FromServices] IUsersService service, [FromBody] UserDto userDto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var targetUser = await service.UpdateAsync(userDto, loggedUser);
            if (targetUser == null) return Results.NotFound();
            return Results.Ok(Response<UserDto>.Ok(targetUser));
        })
        .WithName("UpdateUser")
        .WithDescription("Update user")
        .WithTags("Users")
        .Produces<Response<UserDto>>();  

        app.MapPost("v1/user", async (HttpContext context, [FromServices] IUsersService service, [FromBody] UserDto userDto) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            var targetUser = await service.CreateAsync(userDto, loggedUser);
            if (targetUser == null) return Results.BadRequest();
            return Results.Ok(Response<UserDto>.Ok(targetUser));
        })
        .WithName("CreateUser")
        .WithDescription("Create user")
        .WithTags("Users")
        .Produces<Response<UserDto>>();  

        app.MapDelete("v1/user/{id}", async (HttpContext context, [FromServices] IUsersService service, string id) =>
        {
            var loggedUser = context.GetLoggedUser();
            if (loggedUser == null) return Results.Unauthorized();
            await service.DeleteAsync(id, loggedUser);
            return Results.Ok(Response<bool>.Ok(true));
        })
        .WithName("DeleteUser")
        .WithDescription("Delete user")
        .WithTags("Users")
        .Produces<Response<bool>>();  

        return app;
    }
}