using BillerContracts.Clients;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Clients.Tests;

/// <summary>
/// Shared setup for the typed client tests.
/// </summary>
public static class ClientTestHelper
{
    // Each client gets a different host and path to prove it uses its own URL; ItemUrl has a trailing slash.
    public const string UserUrl = "https://users.test/api/users";
    public const string CustomerUrl = "https://customers.test/api/customers";
    public const string SellerUrl = "https://sellers.test/api/sellers";
    public const string ItemResource = "https://items.test/api/items";
    public const string ItemUrl = ItemResource + "/";
    public const string InvoiceUrl = "https://invoices.test/api/invoices";

    public static readonly Guid Id = Guid.Parse("11111111-2222-3333-4444-555555555555");

    public static ClientsOptions AllUrls() => new()
    {
        UserUrl = UserUrl,
        CustomerUrl = CustomerUrl,
        SellerUrl = SellerUrl,
        ItemUrl = ItemUrl,
        InvoiceUrl = InvoiceUrl,
    };

    public static IOptions<ClientsOptions> Options(Action<ClientsOptions>? change = null)
    {
        ClientsOptions options = AllUrls();
        change?.Invoke(options);
        return Microsoft.Extensions.Options.Options.Create(options);
    }

    /// <summary>
    /// Asserts the single request the handler received.
    /// </summary>
    /// <param name="body">expected JSON body; null when no body is sent, "{}" for any JSON object</param>
    public static void ShouldHaveSent(this FakeHttpMessageHandler handler, HttpMethod method, string url, string? body = null)
    {
        handler.LastRequest.Method.Should().Be(method);
        handler.LastRequest.RequestUri!.AbsoluteUri.Should().Be(url);

        if (body is null)
            handler.LastBody.Should().BeNull();
        else if (body == "{}")
            JToken.Parse(handler.LastBody!).Type.Should().Be(JTokenType.Object);
        else
            JToken.DeepEquals(JToken.Parse(handler.LastBody!), JToken.Parse(body)).Should().BeTrue($"body was {handler.LastBody}");
    }
}
