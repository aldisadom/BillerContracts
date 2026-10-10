using BillerContracts.Responses;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace BillerContracts.Clients;

public class BaseHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _urlBase;

    public BaseHttpClient(IHttpClientFactory httpClientFactory, string urlBase)
    {
        _urlBase = urlBase;
        _httpClientFactory = httpClientFactory;
    }

    public Uri GenerateUrl(string endpoint, Dictionary<string, string>? queryParameters = null)
    {
        StringBuilder urlBuilder = new($"{_urlBase}/{endpoint}");

        if (queryParameters?.Count > 0)
        {
            urlBuilder.Append('?');
            urlBuilder.Append(string.Join("&", queryParameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}")));
        }

        return new Uri(urlBuilder.ToString());
    }

    public static Dictionary<string, string> GenerateQueryFromData<T>(T data)
    {
        var dict = new Dictionary<string, string>();

        if (data == null)
            return dict;

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var value = prop.GetValue(data);
            if (value == null)
                continue;

            string strValue = value switch
            {
                DateTime dt => dt.ToString("o"), // ISO 8601 format
                DateOnly d => d.ToString("yyyy-MM-dd"),
                Guid g => g.ToString(),
                _ => value.ToString() ?? string.Empty
            };

            dict[prop.Name] = strValue;
        }

        return dict;
    }

    public static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headerParameters = null)
    {
        if (headerParameters?.Count > 0)
        {
            foreach (var (key, value) in headerParameters)
            {
                request.Headers.Add(key, value);
            }
        }
    }

    public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParameters = null, Dictionary<string, string>? headerParameters = null)
    {
        HttpResponseMessage response = await SendAsync(HttpMethod.Get, endpoint, queryParameters, headerParameters);
        return DecodeResponse<T>(await ReadBodyAsync(response));
    }

    public async Task<T2> PostAsync<T1, T2>(string endpoint, T1 data, Dictionary<string, string>? queryParameters = null, Dictionary<string, string>? headerParameters = null)
    {
        HttpResponseMessage response = await SendAsync(HttpMethod.Post, endpoint, queryParameters, headerParameters, ToJsonContent(data));
        return DecodeResponse<T2>(await ReadBodyAsync(response));
    }

    public async Task PutAsync<T>(string endpoint, T data, Dictionary<string, string>? queryParameters = null, Dictionary<string, string>? headerParameters = null)
    {
        HttpResponseMessage response = await SendAsync(HttpMethod.Put, endpoint, queryParameters, headerParameters, ToJsonContent(data));
        await ReadBodyAsync(response);
    }

    public async Task DeleteAsync(string endpoint, Dictionary<string, string>? queryParameters = null, Dictionary<string, string>? headerParameters = null)
    {
        HttpResponseMessage response = await SendAsync(HttpMethod.Delete, endpoint, queryParameters, headerParameters);
        await ReadBodyAsync(response);
    }

    public async Task<FileDownloadResult> PostFileDownloadAsync<T>(string endpoint, T data, Dictionary<string, string>? headerParameters = null)
    {
        HttpResponseMessage response = await SendAsync(HttpMethod.Post, endpoint, GenerateQueryFromData(data), headerParameters);

        if (!response.IsSuccessStatusCode)
            await ReadBodyAsync(response);

        var result = new FileDownloadResult
        {
            Content = await response.Content.ReadAsStreamAsync(),
            ContentType = response.Content.Headers.ContentType?.ToString()
        };

        if (response.Content.Headers.ContentDisposition != null)
            result.FileName = response.Content.Headers.ContentDisposition.FileName?.Trim('"');

        else if (response.Content.Headers.TryGetValues("Content-Disposition", out var values))
        {
            var disposition = values.FirstOrDefault();
            if (disposition != null)
            {
                var fileNamePart = "filename=";
                var idx = disposition.IndexOf(fileNamePart, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    result.FileName = disposition.Substring(idx + fileNamePart.Length).Trim('"');
            }
        }

        return result;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, Dictionary<string, string>? queryParameters,
        Dictionary<string, string>? headerParameters, HttpContent? content = null)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        //configure
        HttpRequestMessage request = new(method, GenerateUrl(endpoint, queryParameters))
        {
            Content = content
        };
        AddHeaders(request, headerParameters);

        //send
        return await client.SendAsync(request);
    }

    /// <summary>
    /// Reads the response body, throwing <see cref="ClientAPIException"/> when the status code is not successful.
    /// </summary>
    private async Task<string> ReadBodyAsync(HttpResponseMessage response)
    {
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            DecodeResponseError(responseBody, response);

        return responseBody;
    }

    private static StringContent ToJsonContent<T>(T data) =>
        new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

    private void DecodeResponseError(string body, HttpResponseMessage response)
    {
        string errorMessage = $"Failed to get data from client {_urlBase}, error code: {response.StatusCode}";

        try
        {
            ErrorResponse? error = JsonConvert.DeserializeObject<ErrorResponse>(body);

            if (error is null)
                errorMessage += $", with body: {body}";
            else
                errorMessage += $", with message: {error.Message}";
        }
        catch (Exception)
        {
            errorMessage += $", with body: {body}";
        }

        throw new ClientAPIException(errorMessage);
    }

    private T DecodeResponse<T>(string body)
    {
        return JsonConvert.DeserializeObject<T>(body)
            ?? throw new ClientAPIException($"Failed to deserialize data from client {_urlBase}, with body: {body}");
    }
}
