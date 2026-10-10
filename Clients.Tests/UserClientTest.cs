using BillerContracts.Clients.Clients;
using BillerContracts.Requests.User;
using FluentAssertions;
using static Clients.Tests.ClientTestHelper;

namespace Clients.Tests;

public class UserClientTest
{
    private readonly FakeHttpMessageHandler _handler = new();
    private readonly UserClient _client;

    public UserClientTest()
    {
        _client = new UserClient(Options(), _handler.CreateFactory());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenUserUrlMissing_Throws(string url)
    {
        //Act
        Action act = () => new UserClient(Options(o => o.UserUrl = url), _handler.CreateFactory());

        //Assert
        act.Should().Throw<ArgumentException>().WithMessage("URL is missing UserUrl*");
    }

    [Fact]
    public async Task Login_GivenCredentials_PostsJsonBody()
    {
        //Act
        await _client.Login(new UserLoginRequest { Email = "a@b.c", Password = "pw" });

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, $"{UserUrl}/Login", "{\"Email\":\"a@b.c\",\"Password\":\"pw\"}");
    }

    [Fact]
    public async Task Login_GivenCredentials_DoesNotPutThemInUrlOrHeaders()
    {
        //Act
        await _client.Login(new UserLoginRequest { Email = "a@b.c", Password = "secret" });

        //Assert
        _handler.LastRequest.RequestUri!.Query.Should().BeEmpty();
        _handler.LastRequest.Headers.Should().NotContain(h => h.Key == "Password" || h.Key == "Email");
    }

    [Fact]
    public async Task Get_GivenNoFilter_GetsResourceUrl()
    {
        //Act
        await _client.Get();

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, UserUrl);
    }

    [Fact]
    public async Task Get_GivenId_GetsIdUrl()
    {
        //Act
        await _client.Get(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Get, $"{UserUrl}/{Id}");
    }

    [Fact]
    public async Task Add_GivenRequest_PostsJson()
    {
        //Act
        await _client.Add(new UserAddRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Post, UserUrl, "{}");
    }

    [Fact]
    public async Task Update_GivenRequest_PutsJson()
    {
        //Act
        await _client.Update(new UserUpdateRequest());

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Put, UserUrl, "{}");
    }

    [Fact]
    public async Task Delete_GivenId_DeletesIdUrl()
    {
        //Act
        await _client.Delete(Id);

        //Assert
        _handler.ShouldHaveSent(HttpMethod.Delete, $"{UserUrl}/{Id}");
    }
}
