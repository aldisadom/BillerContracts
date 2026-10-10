using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Seller;
using BillerContracts.Responses;
using BillerContracts.Responses.Seller;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients.Clients;

public class SellerClient : ISellerClient
{
    private readonly BaseHttpClient _httpClient;

    public SellerClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string url = clientOptions.Value.SellerUrl;
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.SellerUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, url);
    }

    public async Task<SellerListResponse> Get()
    {
        return await _httpClient.GetAsync<SellerListResponse>(string.Empty);
    }

    public async Task<SellerListResponse> Get(SellerGetRequest request)
    {
        return await _httpClient.GetAsync<SellerListResponse>(string.Empty, BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<SellerResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<SellerResponse>($"{id}");
    }

    public async Task<AddResponse> Add(SellerAddRequest seller)
    {
        return await _httpClient.PostAsync<SellerAddRequest, AddResponse>(string.Empty, seller);
    }

    public async Task Update(SellerUpdateRequest seller)
    {
        await _httpClient.PutAsync(string.Empty, seller);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{id}");
    }
}
