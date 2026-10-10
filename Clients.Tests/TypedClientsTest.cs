using BillerContracts.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Customer;
using BillerContracts.Requests.Invoice;
using BillerContracts.Requests.Item;
using BillerContracts.Requests.Seller;
using BillerContracts.Requests.User;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Clients.Tests;

public class TypedClientsTest
{
    private const string _urlBase = "https://api.test";
    private static readonly Guid _id = Guid.Parse("11111111-2222-3333-4444-555555555555");

    public delegate Task ClientCall(IHttpClientFactory factory, IOptions<ClientsOptions> options);

    private static readonly IOptions<ClientsOptions> _options = Options.Create(new ClientsOptions { BillioUrl = _urlBase });

    /// <summary>
    /// name, call, expected HTTP method, expected path and query, expected JSON body (null when none is sent)
    /// </summary>
    public static TheoryData<string, ClientCall, string, string, string?> Calls() => new()
    {
        { "User.Login", (f, o) => new UserClient(o, f).Login(new UserLoginRequest { Email = "a@b.c", Password = "pw" }), "POST", "/users/Login", "{\"Email\":\"a@b.c\",\"Password\":\"pw\"}" },
        { "User.Get", (f, o) => new UserClient(o, f).Get(), "GET", "/users", null },
        { "User.GetId", (f, o) => new UserClient(o, f).Get(_id), "GET", $"/users/{_id}", null },
        { "User.Add", (f, o) => new UserClient(o, f).Add(new UserAddRequest()), "POST", "/users", "{}" },
        { "User.Update", (f, o) => new UserClient(o, f).Update(new UserUpdateRequest()), "PUT", "/users", "{}" },
        { "User.Delete", (f, o) => new UserClient(o, f).Delete(_id), "DELETE", $"/users/{_id}", null },

        { "Customer.Get", (f, o) => new CustomerClient(o, f).Get(), "GET", "/customers", null },
        { "Customer.GetQuery", (f, o) => new CustomerClient(o, f).Get(new CustomerGetRequest { SellerId = _id, Page = 2, PageSize = PageSize.p50 }), "GET", $"/customers?SellerId={_id}&Page=2&PageSize=p50", null },
        { "Customer.GetId", (f, o) => new CustomerClient(o, f).Get(_id), "GET", $"/customers/{_id}", null },
        { "Customer.Add", (f, o) => new CustomerClient(o, f).Add(new CustomerAddRequest()), "POST", "/customers", "{}" },
        { "Customer.Update", (f, o) => new CustomerClient(o, f).Update(new CustomerUpdateRequest()), "PUT", "/customers", "{}" },
        { "Customer.Delete", (f, o) => new CustomerClient(o, f).Delete(_id), "DELETE", $"/customers/{_id}", null },

        { "Seller.Get", (f, o) => new SellerClient(o, f).Get(), "GET", "/sellers", null },
        { "Seller.GetQuery", (f, o) => new SellerClient(o, f).Get(new SellerGetRequest { UserId = _id }), "GET", $"/sellers?UserId={_id}&Page=1&PageSize=p25", null },
        { "Seller.GetId", (f, o) => new SellerClient(o, f).Get(_id), "GET", $"/sellers/{_id}", null },
        { "Seller.Add", (f, o) => new SellerClient(o, f).Add(new SellerAddRequest()), "POST", "/sellers", "{}" },
        { "Seller.Update", (f, o) => new SellerClient(o, f).Update(new SellerUpdateRequest()), "PUT", "/sellers", "{}" },
        { "Seller.Delete", (f, o) => new SellerClient(o, f).Delete(_id), "DELETE", $"/sellers/{_id}", null },

        { "Item.Get", (f, o) => new ItemClient(o, f).Get(), "GET", "/items", null },
        { "Item.GetQuery", (f, o) => new ItemClient(o, f).Get(new ItemGetRequest { CustomerId = _id }), "GET", $"/items?CustomerId={_id}&Page=1&PageSize=p25", null },
        { "Item.GetId", (f, o) => new ItemClient(o, f).Get(_id), "GET", $"/items/{_id}", null },
        { "Item.Add", (f, o) => new ItemClient(o, f).Add(new ItemAddRequest { Name = "n", CustomerId = _id }), "POST", "/items", $"{{\"Name\":\"n\",\"Price\":0.0,\"CustomerId\":\"{_id}\",\"Quantity\":0.0}}" },
        { "Item.Update", (f, o) => new ItemClient(o, f).Update(new ItemUpdateRequest()), "PUT", "/items", "{}" },
        { "Item.Delete", (f, o) => new ItemClient(o, f).Delete(_id), "DELETE", $"/items/{_id}", null },

        { "Invoice.Get", (f, o) => new InvoiceClient(o, f).Get(), "GET", "/invoices", null },
        { "Invoice.GetQuery", (f, o) => new InvoiceClient(o, f).Get(new InvoiceGetRequest { CustomerId = _id }), "GET", $"/invoices?CustomerId={_id}&Page=1&PageSize=p25", null },
        { "Invoice.GetId", (f, o) => new InvoiceClient(o, f).Get(_id), "GET", $"/invoices/{_id}", null },
        { "Invoice.Add", (f, o) => new InvoiceClient(o, f).Add(new InvoiceAddRequest()), "POST", "/invoices", "{}" },
        { "Invoice.Update", (f, o) => new InvoiceClient(o, f).Update(new InvoiceUpdateRequest { Seller = new(), Customer = new() }), "PUT", "/invoices", "{}" },
        { "Invoice.UpdateStatus", (f, o) => new InvoiceClient(o, f).UpdateStatus(new InvoiceUpdateStatusRequest { Id = _id, Status = InvoiceStatus.Send }), "PUT", "/invoices/update_status", $"{{\"Id\":\"{_id}\",\"Status\":{(int)InvoiceStatus.Send}}}" },
        { "Invoice.Delete", (f, o) => new InvoiceClient(o, f).Delete(_id), "DELETE", $"/invoices/{_id}", null },
        { "Invoice.GeneratePDF", (f, o) => new InvoiceClient(o, f).GeneratePDF(new InvoiceGenerateRequest { Id = _id, LanguageCode = Language.EN }), "POST", $"/invoices/generate?Id={_id}&LanguageCode=EN&DocumentType=Invoice", null },
    };

    [Theory]
    [MemberData(nameof(Calls))]
    public async Task Call_SendsExpectedRequest(string name, ClientCall call, string method, string pathAndQuery, string? body)
    {
        //Arrange
        var handler = new FakeHttpMessageHandler();

        //Act
        await call(handler.CreateFactory(), _options);

        //Assert
        handler.LastRequest.Method.Method.Should().Be(method, name);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be(pathAndQuery, name);
        handler.LastRequest.RequestUri.GetLeftPart(UriPartial.Authority).Should().Be(_urlBase, name);

        if (body is null)
            handler.LastBody.Should().BeNull(name);
        else if (body == "{}")
            JToken.Parse(handler.LastBody!).Type.Should().Be(JTokenType.Object, name);
        else
            JToken.DeepEquals(JToken.Parse(handler.LastBody!), JToken.Parse(body)).Should().BeTrue($"{name} body was {handler.LastBody}");
    }

    [Fact]
    public async Task Login_DoesNotPutCredentialsInUrl()
    {
        //Arrange
        var handler = new FakeHttpMessageHandler();
        var client = new UserClient(_options, handler.CreateFactory());

        //Act
        await client.Login(new UserLoginRequest { Email = "a@b.c", Password = "secret" });

        //Assert
        handler.LastRequest.RequestUri!.Query.Should().BeEmpty();
        handler.LastRequest.Headers.Should().NotContain(h => h.Key == "Password");
    }

    public static TheoryData<Func<IOptions<ClientsOptions>, object>> Constructors() => new()
    {
        o => new UserClient(o, new FakeHttpMessageHandler().CreateFactory()),
        o => new CustomerClient(o, new FakeHttpMessageHandler().CreateFactory()),
        o => new SellerClient(o, new FakeHttpMessageHandler().CreateFactory()),
        o => new ItemClient(o, new FakeHttpMessageHandler().CreateFactory()),
        o => new InvoiceClient(o, new FakeHttpMessageHandler().CreateFactory()),
    };

    [Theory]
    [MemberData(nameof(Constructors))]
    public void Constructor_GivenMissingUrl_Throws(Func<IOptions<ClientsOptions>, object> create)
    {
        //Act
        Action act = () => create(Options.Create(new ClientsOptions { BillioUrl = " " }));

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing BillioUrl*");
    }
}
