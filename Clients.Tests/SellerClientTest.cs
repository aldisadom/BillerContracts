using BillerContracts.Clients.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Seller;
using FluentAssertions;
using static Clients.Tests.ClientTestHelper;

namespace Clients.Tests;

public class SellerClientTest
{
    private readonly FakeHttpMessageHandler _handler = new();
    private readonly SellerClient _client;

    public SellerClientTest()
    {
        _client = new SellerClient(Options(), _handler.CreateFactory());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenSellerUrlMissing_Throws(string url)
    {
        //Act
        Action act = () => new SellerClient(Options(o => o.SellerUrl = url), _handler.CreateFactory());

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing SellerUrl*");
    }

    [Fact]
    public async Task Get_GivenNoFilter_GetsResourceUrl()
    {
        //Act
        await _client.Get();

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, SellerUrl);
    }

    [Fact]
    public async Task Get_GivenRequest_SendsFieldsAsQuery()
    {
        //Act
        await _client.Get(new SellerGetRequest { UserId = Id });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{SellerUrl}?UserId={Id}&Page=1&PageSize=p25");
    }

    [Fact]
    public async Task Get_GivenId_GetsIdUrl()
    {
        //Act
        await _client.Get(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{SellerUrl}/{Id}");
    }

    [Fact]
    public async Task Add_GivenRequest_PostsJson()
    {
        //Act
        await _client.Add(new SellerAddRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, SellerUrl, "{}");
    }

    [Fact]
    public async Task Update_GivenRequest_PutsJson()
    {
        //Act
        await _client.Update(new SellerUpdateRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, SellerUrl, "{}");
    }

    [Fact]
    public async Task Delete_GivenId_DeletesIdUrl()
    {
        //Act
        await _client.Delete(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Delete, $"{SellerUrl}/{Id}");
    }
}
