using BillerContracts.Clients.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Item;
using FluentAssertions;
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
