using AoTomato.API.Middlewares;

namespace AoTomato.API.Extensions;

public static class ApplicationExtensions
{
    public static WebApplication AddAuthentication(this WebApplication app)
    {
        app.UseMiddleware<PermissionsMiddleware>();
        return app;
    }

    public static WebApplication AddCorsCofiguration(this WebApplication app)
    {
        app.UseCors(option =>
        {
            option.WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS");
            option.AllowAnyHeader();
            option.AllowAnyOrigin();
        });
        return app;
    }
}