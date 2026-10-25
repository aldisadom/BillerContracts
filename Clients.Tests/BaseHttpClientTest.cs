﻿using BillerContracts;
using BillerContracts.Clients;
using BillerContracts.Enums;
using BillerContracts.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Clients.Tests;

public class BaseHttpClientTest
{
    private const string _urlBase = "https://api.test";
    private const string _endpoint = "items";

    public record TestData(int Id, string Name);

    public record QueryData
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public DateTime Time { get; set; }
        public PageSize PageSize { get; set; }
        public int Page { get; set; }
        public string? Missing { get; set; }
    }

    private readonly FakeHttpMessageHandler _handler = new();
    private readonly BaseHttpClient _client;

    public BaseHttpClientTest()
    {
        _client = new BaseHttpClient(_handler.CreateFactory(), _urlBase);
    }

    [Fact]
    public void GenerateUrl_GivenNoQueryParameters_ReturnsBaseWithEndpoint()
    {
        //Act
        Uri result = _client.GenerateUrl(_endpoint);

        //Assert
        result.Should().Be(new Uri($"{_urlBase}/{_endpoint}"));
    }

    [Fact]
    public void GenerateUrl_GivenEmptyQueryParameters_ReturnsBaseWithEndpoint()
    {
        //Act
        Uri result = _client.GenerateUrl(_endpoint, []);

        //Assert
        result.Should().Be(new Uri($"{_urlBase}/{_endpoint}"));
    }

    [Fact]
    public void GenerateUrl_GivenEmptyEndpoint_ReturnsBaseWithoutTrailingSlash()
    {
        //Arrange
        var client = new BaseHttpClient(_handler.CreateFactory(), "https://api.test/v1/items");

        //Act
        Uri result = client.GenerateUrl(string.Empty, new Dictionary<string, string> { ["page"] = "2" });

        //Assert
        result.AbsoluteUri.Should().Be("https://api.test/v1/items?page=2");
    }

    [Fact]
    public void GenerateUrl_GivenBaseWithTrailingSlash_DoesNotDoubleSlash()
    {
        //Arrange
        var client = new BaseHttpClient(_handler.CreateFactory(), "https://api.test/v1/items/");

        //Act
        Uri resource = client.GenerateUrl(string.Empty);
        Uri child = client.GenerateUrl("42");

        //Assert
        resource.AbsoluteUri.Should().Be("https://api.test/v1/items");
        child.AbsoluteUri.Should().Be("https://api.test/v1/items/42");
    }

    [Fact]
    public void GenerateUrl_GivenQueryParameters_AppendsEscapedQuery()
    {
        //Arrange
        var query = new Dictionary<string, string> { ["name"] = "a b&c", ["page"] = "2" };

        //Act
        Uri result = _client.GenerateUrl(_endpoint, query);

        //Assert
        result.AbsoluteUri.Should().Be($"{_urlBase}/{_endpoint}?name=a%20b%26c&page=2");
    }

    [Fact]
    public void GenerateQueryFromData_GivenObject_FormatsEachNonNullProperty()
    {
        //Arrange
        var data = new QueryData
        {
            Id = Guid.Parse("4f7d3a2e-1b2c-4d5e-8f90-123456789abc"),
            Date = new DateOnly(2026, 1, 31),
            Time = new DateTime(2026, 1, 31, 13, 45, 0, DateTimeKind.Utc),
            PageSize = PageSize.p25,
            Page = 3,
        };

        //Act
        Dictionary<string, string> result = BaseHttpClient.GenerateQueryFromData(data);

        //Assert
        result.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["Id"] = "4f7d3a2e-1b2c-4d5e-8f90-123456789abc",
            ["Date"] = "2026-01-31",
            ["Time"] = "2026-01-31T13:45:00.0000000Z",
            ["PageSize"] = "p25",
            ["Page"] = "3",
        });
    }

    [Fact]
    public void GenerateQueryFromData_GivenNull_ReturnsEmpty()
    {
        //Act
        Dictionary<string, string> result = BaseHttpClient.GenerateQueryFromData<QueryData?>(null);

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void AddHeaders_GivenHeaders_AddsAllToRequest()
    {
        //Arrange
        var request = new HttpRequestMessage();
        var headers = new Dictionary<string, string> { ["X-Api-Key"] = "secret", ["X-Trace"] = "1" };

        //Act
        BaseHttpClient.AddHeaders(request, headers);

        //Assert
        request.Headers.GetValues("X-Api-Key").Should().ContainSingle().Which.Should().Be("secret");
        request.Headers.GetValues("X-Trace").Should().ContainSingle().Which.Should().Be("1");
    }

    [Fact]
    public void AddHeaders_GivenNullHeaders_AddsNothing()
    {
        //Arrange
        var request = new HttpRequestMessage();

        //Act
        BaseHttpClient.AddHeaders(request, null);

        //Assert
        request.Headers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_GivenSuccessResponse_ReturnsDeserializedData()
    {
        //Arrange
        var expected = new TestData(1, "item");
        _handler.ResponseBody = JsonConvert.SerializeObject(expected);

        //Act
        TestData result = await _client.GetAsync<TestData>(_endpoint,
            new Dictionary<string, string> { ["id"] = "1" },
            new Dictionary<string, string> { ["X-Api-Key"] = "secret" });

        //Assert
        result.Should().BeEquivalentTo(expected);
        _handler.LastRequest.Method.Should().Be(HttpMethod.Get);
        _handler.LastRequest.RequestUri!.AbsoluteUri.Should().Be($"{_urlBase}/{_endpoint}?id=1");
        _handler.LastRequest.Headers.GetValues("X-Api-Key").Should().ContainSingle().Which.Should().Be("secret");
        _handler.LastBody.Should().BeNull();
    }

    [Fact]
    public async Task PostAsync_GivenSuccessResponse_SendsJsonAndReturnsResponseType()
    {
        //Arrange
        var data = new TestData(2, "posted");
        _handler.StatusCode = HttpStatusCode.Created;
        _handler.ResponseBody = "{\"Id\":7}";

        //Act
        var result = await _client.PostAsync<TestData, Dictionary<string, int>>(_endpoint, data);

        //Assert
        result.Should().ContainKey("Id").WhoseValue.Should().Be(7);
        _handler.LastRequest.Method.Should().Be(HttpMethod.Post);
        _handler.LastRequest.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");
        JsonConvert.DeserializeObject<TestData>(_handler.LastBody!).Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task PutAsync_GivenSuccessResponse_SendsJson()
    {
        //Arrange
        var data = new TestData(3, "put");
        _handler.ResponseBody = "";

        //Act
        await _client.PutAsync(_endpoint, data);

        //Assert
        _handler.LastRequest.Method.Should().Be(HttpMethod.Put);
        _handler.LastRequest.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");
        JsonConvert.DeserializeObject<TestData>(_handler.LastBody!).Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task DeleteAsync_GivenSuccessResponse_SendsDelete()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.NoContent;
        _handler.ResponseBody = "";

        //Act
        await _client.DeleteAsync($"{_endpoint}/1");

        //Assert
        _handler.LastRequest.Method.Should().Be(HttpMethod.Delete);
        _handler.LastRequest.RequestUri!.AbsoluteUri.Should().Be($"{_urlBase}/{_endpoint}/1");
    }

    [Fact]
    public async Task GetAsync_GivenErrorResponseWithMessage_ThrowsWithMessage()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.BadRequest;
        _handler.ResponseBody = "{\"Message\":\"bad input\",\"ExtendedMessage\":\"details\"}";

        //Act
        Func<Task> act = () => _client.GetAsync<TestData>(_endpoint);

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>()
            .WithMessage($"Failed to get data from client {_urlBase}, Error status code: BadRequest, message: bad input, extended message: details");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel("bad input", "details", HttpStatusCode.BadRequest));
    }

    [Fact]
    public async Task GetAsync_GivenNotFoundWithNonJsonBody_ThrowsWithStatusCodeOnly()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.NotFound;
        _handler.ResponseBody = "Not Found";

        //Act
        Func<Task> act = () => _client.GetAsync<TestData>(_endpoint);

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>()
            .WithMessage($"Failed to get data from client {_urlBase}, Error status code: NotFound, with body: Not Found");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel(string.Empty, string.Empty, HttpStatusCode.NotFound));
    }

    [Fact]
    public async Task GetAsync_GivenErrorResponseWithEmptyBody_ThrowsWithBody()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.InternalServerError;
        _handler.ResponseBody = "";

        //Act
        Func<Task> act = () => _client.GetAsync<TestData>(_endpoint);

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>()
            .WithMessage($"Failed to get data from client {_urlBase}, Error status code: InternalServerError, with body: ");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel(string.Empty, string.Empty, HttpStatusCode.InternalServerError));
    }

    [Fact]
    public async Task GetAsync_GivenErrorResponseWithInvalidJson_ThrowsWithBody()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.BadGateway;
        _handler.ResponseBody = "<html>oops</html>";

        //Act
        Func<Task> act = () => _client.GetAsync<TestData>(_endpoint);

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>()
            .WithMessage($"Failed to get data from client {_urlBase}, Error status code: BadGateway, with body: <html>oops</html>");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel(string.Empty, string.Empty, HttpStatusCode.BadGateway));
    }

    [Theory]
    [InlineData("{\"title\":\"One or more validation errors occurred.\",\"status\":400}")]
    [InlineData("{}")]
    public async Task GetAsync_GivenJsonThatIsNotAnErrorResponse_ThrowsWithBody(string body)
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.BadRequest;
        _handler.ResponseBody = body;

        //Act
        Func<Task> act = () => _client.GetAsync<TestData>(_endpoint);

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>()
            .WithMessage($"Failed to get data from client {_urlBase}, Error status code: BadRequest, with body: {body}");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel(string.Empty, string.Empty, HttpStatusCode.BadRequest));
    }

    [Fact]
    public async Task PostAsync_GivenErrorResponse_ThrowsClientApiException()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.Conflict;
        _handler.ResponseBody = "{\"Message\":\"exists\"}";

        //Act
        Func<Task> act = () => _client.PostAsync<TestData, TestData>(_endpoint, new TestData(1, "x"));

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>().WithMessage("*Conflict, message: exists");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel("exists", string.Empty, HttpStatusCode.Conflict));
    }

    [Fact]
    public async Task PutAsync_GivenErrorResponse_ThrowsClientApiException()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.NotFound;
        _handler.ResponseBody = "{\"Message\":\"missing\"}";

        //Act
        Func<Task> act = () => _client.PutAsync(_endpoint, new TestData(1, "x"));

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>().WithMessage("*NotFound, message: missing");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel("missing", string.Empty, HttpStatusCode.NotFound));
    }

    [Fact]
    public async Task DeleteAsync_GivenErrorResponse_ThrowsClientApiException()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.NotFound;
        _handler.ResponseBody = "{\"Message\":\"missing\"}";

        //Act
        Func<Task> act = () => _client.DeleteAsync($"{_endpoint}/1");

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>().WithMessage("*NotFound, message: missing");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel("missing", string.Empty, HttpStatusCode.NotFound));
    }

    [Fact]
    public async Task GetAsync_GivenNullBody_ThrowsDeserializeException()
    {
        //Arrange
        _handler.ResponseBody = "null";

        //Act
        Func<Task> act = () => _client.GetAsync<TestData>(_endpoint);

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>()
            .WithMessage($"Failed to deserialize data from client {_urlBase}, with body: null");
        exception.Which.Error.Should().BeNull();
    }

    [Fact]
    public async Task PostFileDownloadAsync_GivenContentDisposition_ReturnsFileWithName()
    {
        //Arrange
        _handler.ResponseBody = "%PDF";
        _handler.ConfigureContentHeaders = h =>
        {
            h.ContentType = new MediaTypeHeaderValue("application/pdf");
            h.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "\"invoice.pdf\"" };
        };
        var data = new QueryData { Page = 1 };

        //Act
        FileDownloadResponse result = await _client.PostFileDownloadAsync(_endpoint, data);

        //Assert
        result.FileName.Should().Be("invoice.pdf");
        result.ContentType.Should().Be("application/pdf");
        new StreamReader(result.Content).ReadToEnd().Should().Be("%PDF");
        _handler.LastRequest.Method.Should().Be(HttpMethod.Post);
        _handler.LastRequest.RequestUri!.Query.Should().Contain("Page=1");
        _handler.LastBody.Should().BeNull();
    }

    [Fact]
    public async Task PostFileDownloadAsync_GivenNoContentDisposition_ReturnsFileWithoutName()
    {
        //Arrange
        _handler.ResponseBody = "%PDF";

        //Act
        FileDownloadResponse result = await _client.PostFileDownloadAsync(_endpoint, new QueryData());

        //Assert
        result.FileName.Should().BeNull();
        new StreamReader(result.Content).ReadToEnd().Should().Be("%PDF");
    }

    [Fact]
    public async Task PostFileDownloadAsync_GivenErrorResponse_ThrowsClientApiException()
    {
        //Arrange
        _handler.StatusCode = HttpStatusCode.NotFound;
        _handler.ResponseBody = "{\"Message\":\"no invoice\"}";

        //Act
        Func<Task> act = () => _client.PostFileDownloadAsync(_endpoint, new QueryData());

        //Assert
        var exception = await act.Should().ThrowAsync<ClientAPIException>().WithMessage("*NotFound, message: no invoice");
        exception.Which.Error.Should().BeEquivalentTo(new ErrorModel("no invoice", string.Empty, HttpStatusCode.NotFound));
    }
}
