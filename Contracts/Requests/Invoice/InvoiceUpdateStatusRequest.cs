using BillerContracts.Enums;

namespace BillerContracts.Requests.Invoice;

public class InvoiceUpdateStatusRequest
{
    public Guid Id { get; set; }
    public InvoiceStatus Status { get; set; }
}
