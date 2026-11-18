using BillerContracts.Clients.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Item;
using BillerContracts.Responses.Item;
using AwesomeAssertions;
using static Clients.Tests.ClientTestHelper;

namespace Clients.Tests;

public class ItemClientTest
{
    private readonly FakeHttpMessageHandler _handler = new();
    private readonly ItemClient _client;

    public ItemClientTest()
    {
        _client = new ItemClient(Options(), _handler.CreateFactory());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenItemUrlMissing_Throws(string url)
    {
        //Act
        Action act = () => new ItemClient(Options(o => o.ItemUrl = url), _handler.CreateFactory());

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing ItemUrl*");
    }

    [Fact]
    public async Task Get_GivenNoFilter_GetsResourceUrl()
    {
        //Act
        await _client.Get();

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, ItemResource);
    }

    [Fact]
    public async Task Get_GivenRequest_SendsFieldsAsQuery()
    {
        //Act
        await _client.Get(new ItemGetRequest { CustomerId = Id });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{ItemResource}?CustomerId={Id}&Page=1&PageSize=p25");
    }

    [Fact]
    public async Task Get_GivenId_GetsIdUrl()
    {
        //Act
        await _client.Get(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{ItemResource}/{Id}");
    }

    [Fact]
    public async Task GetVersions_GivenId_GetsVersionsUrl()
    {
        //Act
        await _client.GetVersions(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{ItemResource}/{Id}/versions");
    }

    [Fact]
    public async Task GetVersions_GivenResponse_ReturnsVersions()
    {
        //Arrange
        var expected = new ItemVersionListResponse
        {
            Versions = [new ItemVersionResponse { ItemId = Id, Version = 2, Name = "Iron throne", Price = 60m, ChangedDate = new DateTime(2026, 11, 10, 14, 30, 0, DateTimeKind.Utc) }]
        };
        _handler.ResponseBody = $"{{\"versions\":[{{\"itemId\":\"{Id}\",\"version\":2,\"name\":\"Iron throne\",\"price\":60,\"changedDate\":\"2026-11-10T14:30:00Z\"}}]}}";

        //Act
        ItemVersionListResponse result = await _client.GetVersions(Id);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_GivenId_ReturnsItemWithVersion()
    {
        //Arrange
        var expected = new ItemResponse { Id = Id, CustomerId = Id, Name = "Iron throne", Price = 60m, Quantity = 1m, Version = 2 };
        _handler.ResponseBody = $"{{\"id\":\"{Id}\",\"customerId\":\"{Id}\",\"name\":\"Iron throne\",\"price\":60,\"quantity\":1,\"version\":2}}";

        //Act
        ItemResponse result = await _client.Get(Id);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Add_GivenRequest_PostsJson()
    {
        //Act
        await _client.Add(new ItemAddRequest { Name = "n", CustomerId = Id });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, ItemResource, $"{{\"Name\":\"n\",\"Price\":0.0,\"CustomerId\":\"{Id}\",\"Quantity\":0.0}}");
    }

    [Fact]
    public async Task Update_GivenRequest_PutsJson()
    {
        //Act
        await _client.Update(new ItemUpdateRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, ItemResource, "{}");
    }

    [Fact]
    public async Task Delete_GivenId_DeletesIdUrl()
    {
        //Act
        await _client.Delete(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Delete, $"{ItemResource}/{Id}");
    }
}
