using BillerContracts.Enums;

namespace BillerContracts.Requests.Invoice;

public record InvoiceGetRequest
{
    public Guid? UserId { get; set; }
    public Guid? SellerId { get; set; }
    public Guid? CustomerId { get; set; }
    public int Page { get; set; } = 1;
    public PageSize PageSize { get; set; } = PageSize.p25;
}
