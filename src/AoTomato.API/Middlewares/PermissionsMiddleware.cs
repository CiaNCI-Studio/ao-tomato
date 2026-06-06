using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using AoTomato.API.Extensions;
using AoTomato.API.Helpers;
using AoTomato.domain.Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace AoTomato.API.Middlewares;

public class PermissionsMiddleware
{
    private readonly RequestDelegate next;

    public PermissionsMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var result = ValidateAccess(context);
        if (string.IsNullOrEmpty(result))
        {
            await next(context);
        }
        else
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.ContentType = "application/json";
            var writer = context.Response.BodyWriter;
            await writer.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { error = result })));
            await writer.CompleteAsync();
        }
    }

    private static string ValidateAccess(HttpContext context)
    {
        if (!ValidateToken(context))
        {
            Log.Logger.Warning("Permission denied invalid JWT token: {token}", context.GetToken());
            return "Permission denied invalid JWT token.";
        }
        return string.Empty;
    }

    private static bool ValidateToken(HttpContext context)
    {
        if (context.GetEndpoint() is Endpoint endpoint)
        {
            var settings = ServiceProviderFactory.ServiceProvider?.GetRequiredService<AuthSettings>();
            if (endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null || settings == null)
            {
                return true;
            }
            var token = context.GetToken();
            if (token == null)
            {
                Log.Logger.Warning("JWT token validation failed: Missing token");
                return false;
            }
            var key = Encoding.ASCII.GetBytes(settings?.SecretKey ?? string.Empty);
            var audience = settings?.Audience;
            var issuer = settings?.Issuer;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = !string.IsNullOrEmpty(issuer),
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrEmpty(audience),
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
            }
            catch (Exception ex)
            {
                Log.Logger.Warning("JWT token validation failed: {message}", ex.Message);
                return false;
            }
        }
        return true;
    }
}