using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Seller;
using BillerContracts.Responses;
using BillerContracts.Responses.Seller;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients;

public class SellerClient : ISellerClient
{
    private readonly BaseHttpClient _httpClient;
    private readonly string _controller = "sellers";

    public SellerClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string billioUrl = clientOptions.Value.BillioUrl;
        if (string.IsNullOrWhiteSpace(billioUrl))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.BillioUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, billioUrl);
    }

    public async Task<SellerListResponse> Get()
    {
        return await _httpClient.GetAsync<SellerListResponse>($"{_controller}");
    }

    public async Task<SellerListResponse> Get(SellerGetRequest request)
    {
        return await _httpClient.GetAsync<SellerListResponse>($"{_controller}", BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<SellerResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<SellerResponse>($"{_controller}/{id}");
    }

    public async Task<AddResponse> Add(SellerAddRequest seller)
    {
        return await _httpClient.PostAsync<SellerAddRequest, AddResponse>($"{_controller}", seller);
    }

    public async Task Update(SellerUpdateRequest seller)
    {
        await _httpClient.PutAsync($"{_controller}", seller);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{_controller}/{id}");
    }
}
