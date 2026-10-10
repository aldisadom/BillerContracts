using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Customer;
using BillerContracts.Responses;
using BillerContracts.Responses.Customer;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients;

public class CustomerClient : ICustomerClient
{
    private readonly BaseHttpClient _httpClient;
    private readonly string _controller = "customers";

    public CustomerClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string billioUrl = clientOptions.Value.BillioUrl;
        if (string.IsNullOrWhiteSpace(billioUrl))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.BillioUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, billioUrl);
    }

    public async Task<CustomerListResponse> Get()
    {
        return await _httpClient.GetAsync<CustomerListResponse>($"{_controller}");
    }

    public async Task<CustomerListResponse> Get(CustomerGetRequest request)
    {
        return await _httpClient.GetAsync<CustomerListResponse>($"{_controller}", BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<CustomerResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<CustomerResponse>($"{_controller}/{id}");
    }

    public async Task<AddResponse> Add(CustomerAddRequest customer)
    {
        return await _httpClient.PostAsync<CustomerAddRequest, AddResponse>($"{_controller}", customer);
    }

    public async Task Update(CustomerUpdateRequest customer)
    {
        await _httpClient.PutAsync($"{_controller}", customer);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{_controller}/{id}");
    }
}
