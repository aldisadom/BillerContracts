using BillerContracts.Requests.Invoice;
using BillerContracts.Responses;
using BillerContracts.Responses.Invoice;

namespace BillerContracts.Clients.Interfaces;

public interface IInvoiceClient
{
    Task<AddResponse> Add(InvoiceAddRequest invoice);
    Task Delete(Guid id);
    Task<InvoiceListResponse> Get();
    Task<InvoiceListResponse> Get(InvoiceGetRequest request);
    Task<InvoiceResponse> Get(Guid id);
    Task Update(InvoiceUpdateRequest invoice);
    Task UpdateStatus(InvoiceUpdateStatusRequest invoice);
    Task<FileDownloadResult> GeneratePDF(InvoiceGenerateRequest invoice);
}
