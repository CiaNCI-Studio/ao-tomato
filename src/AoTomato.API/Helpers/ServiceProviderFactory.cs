namespace AoTomato.API.Helpers;

public static class ServiceProviderFactory
{
    private static ServiceProvider? serviceProvider;

    public static ServiceProvider? ServiceProvider => serviceProvider;

    internal static void SetServiceProvider(ServiceProvider provider)
    {
        serviceProvider = provider;
    }
}