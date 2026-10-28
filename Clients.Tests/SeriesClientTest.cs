using BillerContracts.Clients.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Series;
using FluentAssertions;
using static Clients.Tests.ClientTestHelper;

namespace Clients.Tests;

public class SeriesClientTest
{
    private readonly FakeHttpMessageHandler _handler = new();
    private readonly SeriesClient _client;

    public SeriesClientTest()
    {
        _client = new SeriesClient(Options(), _handler.CreateFactory());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenSeriesUrlMissing_Throws(string url)
    {
        //Act
        Action act = () => new SeriesClient(Options(o => o.SeriesUrl = url), _handler.CreateFactory());

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing SeriesUrl*");
    }

    [Fact]
    public async Task Get_GivenNoFilter_GetsResourceUrl()
    {
        //Act
        await _client.Get();

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, SeriesUrl);
    }

    [Fact]
    public async Task Get_GivenRequest_SendsFieldsAsQuery()
    {
        //Act
        await _client.Get(new SeriesGetRequest { SellerId = Id, Page = 2, PageSize = PageSize.p50 });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{SeriesUrl}?SellerId={Id}&Page=2&PageSize=p50");
    }

    [Fact]
    public async Task Get_GivenId_GetsIdUrl()
    {
        //Act
        await _client.Get(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{SeriesUrl}/{Id}");
    }

    [Fact]
    public async Task Add_GivenRequest_PostsJson()
    {
        //Act
        await _client.Add(new SeriesAddRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, SeriesUrl, "{}");
    }

    [Fact]
    public async Task Update_GivenRequest_PutsJson()
    {
        //Act
        await _client.Update(new SeriesUpdateRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, SeriesUrl, "{}");
    }

    [Fact]
    public async Task Delete_GivenId_DeletesIdUrl()
    {
        //Act
        await _client.Delete(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Delete, $"{SeriesUrl}/{Id}");
    }
}
