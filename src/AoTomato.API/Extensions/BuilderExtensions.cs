namespace AoTomato.API.Extensions;

public static class BuilderExtensions
{
   public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });
        return builder;
    }
}