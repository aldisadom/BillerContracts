using BillerContracts.Clients.Interfaces;
using BillerContracts.Requests.Invoice;
using BillerContracts.Responses;
using BillerContracts.Responses.Invoice;
using Microsoft.Extensions.Options;

namespace BillerContracts.Clients;

public class InvoiceClient : IInvoiceClient
{
    private readonly BaseHttpClient _httpClient;
    private readonly string _controller = "invoices";

    public InvoiceClient(IOptions<ClientsOptions> clientOptions, IHttpClientFactory httpClientFactory)
    {
        string billioUrl = clientOptions.Value.BillioUrl;
        if (string.IsNullOrWhiteSpace(billioUrl))
            throw new ArgumentException($"URL is missing {nameof(ClientsOptions.BillioUrl)}", nameof(clientOptions));

        _httpClient = new(httpClientFactory, billioUrl);
    }

    public async Task<InvoiceListResponse> Get()
    {
        return await _httpClient.GetAsync<InvoiceListResponse>($"{_controller}");
    }

    public async Task<InvoiceListResponse> Get(InvoiceGetRequest request)
    {
        return await _httpClient.GetAsync<InvoiceListResponse>($"{_controller}", BaseHttpClient.GenerateQueryFromData(request));
    }

    public async Task<InvoiceResponse> Get(Guid id)
    {
        return await _httpClient.GetAsync<InvoiceResponse>($"{_controller}/{id}");
    }

    public async Task<AddResponse> Add(InvoiceAddRequest invoice)
    {
        return await _httpClient.PostAsync<InvoiceAddRequest, AddResponse>($"{_controller}", invoice);
    }

    public async Task Update(InvoiceUpdateRequest invoice)
    {
        await _httpClient.PutAsync($"{_controller}", invoice);
    }

    public async Task UpdateStatus(InvoiceUpdateStatusRequest invoice)
    {
        await _httpClient.PutAsync($"{_controller}/update_status", invoice);
    }

    public async Task Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"{_controller}/{id}");
    }

    public async Task<FileDownloadResult> GeneratePDF(InvoiceGenerateRequest invoice)
    {
        return await _httpClient.PostFileDownloadAsync($"{_controller}/generate", invoice);
    }
}
