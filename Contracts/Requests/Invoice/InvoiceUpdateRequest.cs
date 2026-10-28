using BillerContracts.Requests.Customer;
using BillerContracts.Requests.Seller;

namespace BillerContracts.Requests.Invoice;

public class InvoiceUpdateRequest
{
    public Guid Id { get; set; }
    public int InvoiceNumber { get; set; }
    public Guid SellerId { get; set; }
    public Guid CustomerId { get; set; }
    public List<InvoiceItemRequest> Items { get; set; } = [];
    public string? Comments { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}
