using BillerContracts.Clients;
using BillerContracts.Clients.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Clients.Tests;

public class DependencyInjectionTests
{
    private static readonly Dictionary<Type, Type> _expectedClients = new()
    {
        [typeof(IUserClient)] = typeof(UserClient),
        [typeof(ICustomerClient)] = typeof(CustomerClient),
        [typeof(ISellerClient)] = typeof(SellerClient),
        [typeof(IItemClient)] = typeof(ItemClient),
        [typeof(IInvoiceClient)] = typeof(InvoiceClient),
    };

    [Fact]
    public void AddBillerClients_GivenConfiguration_RegistersAllClientsAndBindsOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["BillioUrl"] = "https://api.test" })
            .Build();
        var services = new ServiceCollection();

        // Act
        var result = services.AddBillerClients(configuration);
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        // Assert
        using var _ = new AssertionScope();

        result.Should().BeSameAs(services);
        scope.ServiceProvider.GetRequiredService<IOptions<ClientsOptions>>().Value.BillioUrl.Should().Be("https://api.test");
        scope.ServiceProvider.GetService<IHttpClientFactory>().Should().NotBeNull();

        foreach (var (serviceType, implementationType) in _expectedClients)
        {
            services.Should().ContainSingle(d => d.ServiceType == serviceType)
                .Which.Lifetime.Should().Be(ServiceLifetime.Scoped, "{0} should be scoped", serviceType.Name);

            scope.ServiceProvider.GetService(serviceType)
                .Should().BeOfType(implementationType, "{0} should resolve to {1}", serviceType.Name, implementationType.Name);
        }
    }
}
