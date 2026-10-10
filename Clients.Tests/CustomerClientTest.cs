using BillerContracts.Clients.Clients;
using BillerContracts.Enums;
using BillerContracts.Requests.Customer;
using FluentAssertions;
using static Clients.Tests.ClientTestHelper;

namespace Clients.Tests;

public class CustomerClientTest
{
    private readonly FakeHttpMessageHandler _handler = new();
    private readonly CustomerClient _client;

    public CustomerClientTest()
    {
        _client = new CustomerClient(Options(), _handler.CreateFactory());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenCustomerUrlMissing_Throws(string url)
    {
        //Act
        Action act = () => new CustomerClient(Options(o => o.CustomerUrl = url), _handler.CreateFactory());

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing CustomerUrl*");
    }

    [Fact]
    public async Task Get_GivenNoFilter_GetsResourceUrl()
    {
        //Act
        await _client.Get();

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, CustomerUrl);
    }

    [Fact]
    public async Task Get_GivenRequest_SendsFieldsAsQuery()
    {
        //Act
        await _client.Get(new CustomerGetRequest { SellerId = Id, Page = 2, PageSize = PageSize.p50 });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{CustomerUrl}?SellerId={Id}&Page=2&PageSize=p50");
    }

    [Fact]
    public async Task Get_GivenId_GetsIdUrl()
    {
        //Act
        await _client.Get(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{CustomerUrl}/{Id}");
    }

    [Fact]
    public async Task Add_GivenRequest_PostsJson()
    {
        //Act
        await _client.Add(new CustomerAddRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, CustomerUrl, "{}");
    }

    [Fact]
    public async Task Update_GivenRequest_PutsJson()
    {
        //Act
        await _client.Update(new CustomerUpdateRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, CustomerUrl, "{}");
    }

    [Fact]
    public async Task Delete_GivenId_DeletesIdUrl()
    {
        //Act
        await _client.Delete(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Delete, $"{CustomerUrl}/{Id}");
    }
}
