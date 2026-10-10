using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Item;
using BillerContracts.Responses;
using BillerContracts.Responses.Item;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients;

public class ItemClient : IItemClient
{
    private readonly BaseHttpClient _httpClient;
    private readonly string _controller = "items";

    public ItemClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string billioUrl = clientOptions.Value.BillioUrl;
        if (string.IsNullOrWhiteSpace(billioUrl))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.BillioUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, billioUrl);
    }

    public async Task<ItemListResponse> Get()
    {
        return await _httpClient.GetAsync<ItemListResponse>($"{_controller}");
    }

    public async Task<ItemListResponse> Get(ItemGetRequest request)
    {
        return await _httpClient.GetAsync<ItemListResponse>($"{_controller}", BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<ItemResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<ItemResponse>($"{_controller}/{id}");
    }

    public async Task<AddResponse> Add(ItemAddRequest item)
    {
        return await _httpClient.PostAsync<ItemAddRequest, AddResponse>($"{_controller}", item);
    }

    public async Task Update(ItemUpdateRequest item)
    {
        await _httpClient.PutAsync($"{_controller}", item);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{_controller}/{id}");
    }
}
