using BillerContracts.Clients.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Invoice;
using BillerContracts.Responses.Invoice;
using AwesomeAssertions;
using Newtonsoft.Json.Linq;
using static Clients.Tests.ClientTestHelper;

namespace Clients.Tests;

public class InvoiceClientTest
{
    private readonly FakeHttpMessageHandler _handler = new();
    private readonly InvoiceClient _client;

    public InvoiceClientTest()
    {
        _client = new InvoiceClient(Options(), _handler.CreateFactory());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenInvoiceUrlMissing_Throws(string url)
    {
        //Act
        Action act = () => new InvoiceClient(Options(o => o.InvoiceUrl = url), _handler.CreateFactory());

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing InvoiceUrl*");
    }

    [Fact]
    public async Task Get_GivenNoFilter_GetsResourceUrl()
    {
        //Act
        await _client.Get();

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, InvoiceUrl);
    }

    [Fact]
    public async Task Get_GivenRequest_SendsFieldsAsQuery()
    {
        //Act
        await _client.Get(new InvoiceGetRequest { CustomerId = Id });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{InvoiceUrl}?CustomerId={Id}&Page=1&PageSize=p25");
    }

    [Fact]
    public async Task Get_GivenId_GetsIdUrl()
    {
        //Act
        await _client.Get(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{InvoiceUrl}/{Id}");
    }

    [Fact]
    public async Task Get_GivenLineWithVersion_ReturnsLineVersion()
    {
        //Arrange
        var expected = new InvoiceItemResponse { Id = Id, Name = "Iron throne", Price = 50m, Quantity = 2m, Comments = "c", Version = 3 };
        _handler.ResponseBody = $"{{\"items\":[{{\"id\":\"{Id}\",\"name\":\"Iron throne\",\"price\":50,\"quantity\":2,\"comments\":\"c\",\"version\":3}}]}}";

        //Act
        InvoiceResponse result = await _client.Get(Id);

        //Assert
        result.Items.Should().ContainSingle().Which.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Add_GivenRequest_PostsJson()
    {
        //Act
        await _client.Add(new InvoiceAddRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, InvoiceUrl, "{}");
    }

    [Fact]
    public async Task Update_GivenRequest_PutsJson()
    {
        //Act
        await _client.Update(new InvoiceUpdateRequest { SellerId = new(), CustomerId = new() });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, InvoiceUrl, "{}");
    }

    [Fact]
    public async Task Update_GivenLineWithVersion_SendsLineVersion()
    {
        //Act
        await _client.Update(new InvoiceUpdateRequest { Items = [new InvoiceItemRequest { Id = Id, Quantity = 2m, Version = 3 }] });

        //Assert
        JToken.Parse(_handler.LastBody!)["Items"]![0]!["Version"]!.Value<int>().Should().Be(3);
    }

    [Fact]
    public async Task UpdateStatus_GivenRequest_PutsJsonToUpdateStatus()
    {
        //Act
        await _client.UpdateStatus(new InvoiceUpdateStatusRequest { Id = Id, Status = InvoiceStatus.Send });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, $"{InvoiceUrl}/update_status", $"{{\"Id\":\"{Id}\",\"Status\":{(int)InvoiceStatus.Send}}}");
    }

    [Fact]
    public async Task Delete_GivenId_DeletesIdUrl()
    {
        //Act
        await _client.Delete(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Delete, $"{InvoiceUrl}/{Id}");
    }

    [Fact]
    public async Task GeneratePDF_GivenRequest_PostsFieldsAsQuery()
    {
        //Act
        await _client.GeneratePDF(new InvoiceGenerateRequest { Id = Id, LanguageCode = Language.EN });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, $"{InvoiceUrl}/generate?Id={Id}&LanguageCode=EN&DocumentType=Invoice");
    }
}
