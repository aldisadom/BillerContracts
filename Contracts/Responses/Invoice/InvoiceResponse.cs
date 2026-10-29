using BillerContracts.Enums;

namespace BillerContracts.Responses.Invoice;

public class InvoiceResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SeriesId { get; set; }
    public string SeriesName { get; set; } = string.Empty;
    public int InvoiceNumber { get; set; }
    public DateOnly CreatedDate { get; set; }
    public DateOnly DueDate { get; set; }
    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<InvoiceItemResponse>? Items { get; set; }
    public string? Comments { get; set; }
    public decimal TotalPrice { get; set; }
    public InvoiceStatus Status { get; set; }
}
