namespace BillerContracts.Responses.Invoice;

public record InvoiceListResponse : PagedResponse
{
    public List<InvoiceResponse> Invoices { get; set; } = [];
}
