using System.Net;
using System.Net.Http.Headers;

namespace Clients.Tests;

/// <summary>
/// Records every request it receives and replies with a configured response.
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    public List<HttpRequestMessage> Requests { get; } = [];
    public List<string?> RequestBodies { get; } = [];

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public string ResponseBody { get; set; } = "{}";
    public Action<HttpContentHeaders>? ConfigureContentHeaders { get; set; }

    public HttpRequestMessage LastRequest => Requests.Single();
    public string? LastBody => RequestBodies.Single();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        RequestBodies.Add(request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken));

        var content = new StringContent(ResponseBody);
        ConfigureContentHeaders?.Invoke(content.Headers);

        return new HttpResponseMessage(StatusCode) { Content = content };
    }

    public IHttpClientFactory CreateFactory()
    {
        var factory = new Moq.Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(Moq.It.IsAny<string>())).Returns(() => new HttpClient(this, disposeHandler: false));
        return factory.Object;
    }
}
