using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using AoTomato.Domain.Login.Dtos;
using AoTomato.Domain.Users.Dtos;

namespace AoTomato.API.Extensions;

public static class HttpContextExtensions
{
    public static UserDto? GetUser(this HttpContext context)
    {
        if (context.Items.ContainsKey("user"))
        {
            return (UserDto?)context.Items["user"];
        }
        var token = context.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            if (jwtSecurityToken.Payload.TryGetValue("user", out var userObject))
            {
                var user = JsonSerializer.Deserialize<UserDto>(userObject.ToString() ?? "", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                context.Items.Add("user", user);
                return user;
            }
        }
        return null;
    }

    public static string? GetToken(this HttpContext context, bool clear = true)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault();
        if (clear)
        {
            token = token?.Replace("bearer ", "", StringComparison.InvariantCultureIgnoreCase);
            token = token?.Replace("jwt ", "", StringComparison.InvariantCultureIgnoreCase);
        }
        return token;
    }

    public static string? GetApiKey(this HttpContext context)
    {
        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            apiKey = context.Request.Query["api_key"].FirstOrDefault();
        }
        return apiKey;
    }

    public static LoggedUserDto? GetLoggedUser(this HttpContext context, bool clear = true)
    {
        return new LoggedUserDto(context.GetUser(), context.GetToken(clear) ?? string.Empty);
    }
}