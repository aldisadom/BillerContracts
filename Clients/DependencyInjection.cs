using BillerContracts.Clients.Clients;
using BillerContracts.Clients.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillerContracts.Clients;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the Biller API clients. Configure <see cref="ClientsOptions"/> separately,
    /// or use the overload that binds it from configuration.
    /// </summary>
    private static IServiceCollection AddBillerClients(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddScoped<IUserClient, UserClient>();
        services.AddScoped<IItemClient, ItemClient>();
        services.AddScoped<ICustomerClient, CustomerClient>();
        services.AddScoped<ISellerClient, SellerClient>();
        services.AddScoped<IInvoiceClient, InvoiceClient>();
        services.AddScoped<ISeriesClient, SeriesClient>();

        return services;
    }

    /// <summary>
    /// Registers the Biller API clients and binds <see cref="ClientsOptions"/> from <paramref name="configuration"/>.
    /// </summary>
    public static IServiceCollection AddBillerClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientsOptions>(configuration);
        return services.AddBillerClients();
    }
}
