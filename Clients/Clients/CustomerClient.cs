using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Customer;
using BillerContracts.Responses;
using BillerContracts.Responses.Customer;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients.Clients;

public class CustomerClient : ICustomerClient
{
    private readonly BaseHttpClient _httpClient;

    public CustomerClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string url = clientOptions.Value.CustomerUrl;
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.CustomerUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, url);
    }

    public async Task<CustomerListResponse> Get()
    {
        return await _httpClient.GetAsync<CustomerListResponse>(string.Empty);
    }

    public async Task<CustomerListResponse> Get(CustomerGetRequest request)
    {
        return await _httpClient.GetAsync<CustomerListResponse>(string.Empty, BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<CustomerResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<CustomerResponse>($"{id}");
    }

    public async Task<AddResponse> Add(CustomerAddRequest customer)
    {
        return await _httpClient.PostAsync<CustomerAddRequest, AddResponse>(string.Empty, customer);
    }

    public async Task Update(CustomerUpdateRequest customer)
    {
        await _httpClient.PutAsync(string.Empty, customer);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{id}");
    }
}
