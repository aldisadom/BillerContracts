using BillerContracts.Enums;

namespace BillerContracts.Requests.Item;

public record ItemGetRequest
{
    public Guid? CustomerId { get; set; }
    public int Page { get; set; } = 1;
    public PageSize PageSize { get; set; } = PageSize.p25;
}
