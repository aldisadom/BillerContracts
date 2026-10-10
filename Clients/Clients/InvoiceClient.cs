using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Invoice;
using BillerContracts.Responses;
using BillerContracts.Responses.Invoice;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients.Clients;

public class InvoiceClient : IInvoiceClient
{
    private readonly BaseHttpClient _httpClient;

    public InvoiceClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string url = clientOptions.Value.InvoiceUrl;
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.InvoiceUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, url);
    }

    public async Task<InvoiceListResponse> Get()
    {
        return await _httpClient.GetAsync<InvoiceListResponse>(string.Empty);
    }

    public async Task<InvoiceListResponse> Get(InvoiceGetRequest request)
    {
        return await _httpClient.GetAsync<InvoiceListResponse>(string.Empty, BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<InvoiceResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<InvoiceResponse>($"{id}");
    }

    public async Task<AddResponse> Add(InvoiceAddRequest invoice)
    {
        return await _httpClient.PostAsync<InvoiceAddRequest, AddResponse>(string.Empty, invoice);
    }

    public async Task Update(InvoiceUpdateRequest invoice)
    {
        await _httpClient.PutAsync(string.Empty, invoice);
    }

    public async Task UpdateStatus(InvoiceUpdateStatusRequest invoice)
    {
        await _httpClient.PutAsync("update_status", invoice);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{id}");
    }

    public async Task<FileDownloadResponse> GeneratePDF(InvoiceGenerateRequest invoice)
    {
        return await _httpClient.PostFileDownloadAsync("generate", invoice);
    }
}
