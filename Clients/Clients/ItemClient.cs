using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Item;
using BillerContracts.Responses;
using BillerContracts.Responses.Item;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients.Clients;

public class ItemClient : IItemClient
{
    private readonly BaseHttpClient _httpClient;

    public ItemClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string url = clientOptions.Value.ItemUrl;
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.ItemUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, url);
    }

    public async Task<ItemListResponse> Get()
    {
        return await _httpClient.GetAsync<ItemListResponse>(string.Empty);
    }

    public async Task<ItemListResponse> Get(ItemGetRequest request)
    {
        return await _httpClient.GetAsync<ItemListResponse>(string.Empty, BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<ItemResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<ItemResponse>($"{id}");
    }

    public async Task<AddResponse> Add(ItemAddRequest item)
    {
        return await _httpClient.PostAsync<ItemAddRequest, AddResponse>(string.Empty, item);
    }

    public async Task Update(ItemUpdateRequest item)
    {
        await _httpClient.PutAsync(string.Empty, item);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{id}");
    }
}
