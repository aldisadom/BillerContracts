using BillerContracts.Enums;

namespace BillerContracts.Responses.Invoice;

public record InvoiceListResponse
{
    public List<InvoiceResponse> Invoices { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
