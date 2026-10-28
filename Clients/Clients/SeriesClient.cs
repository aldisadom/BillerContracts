using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Series;
using BillerContracts.Responses;
using BillerContracts.Responses.Series;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients.Clients;

public class SeriesClient : ISeriesClient
{
    private readonly BaseHttpClient _httpClient;

    public SeriesClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string url = clientOptions.Value.SeriesUrl;
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.SeriesUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, url);
    }

    public async Task<SeriesListResponse> Get()
    {
        return await _httpClient.GetAsync<SeriesListResponse>(string.Empty);
    }

    public async Task<SeriesListResponse> Get(SeriesGetRequest request)
    {
        return await _httpClient.GetAsync<SeriesListResponse>(string.Empty, BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<SeriesResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<SeriesResponse>($"{id}");
    }

    public async Task<AddResponse> Add(SeriesAddRequest series)
    {
        return await _httpClient.PostAsync<SeriesAddRequest, AddResponse>(string.Empty, series);
    }

    public async Task Update(SeriesUpdateRequest series)
    {
        await _httpClient.PutAsync(string.Empty, series);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{id}");
    }
}
