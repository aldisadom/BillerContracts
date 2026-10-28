using BillerContracts.Enums;

namespace BillerContracts.Requests.Series;

public record SeriesGetRequest
{
    public Guid? SellerId { get; set; }
    public int Page { get; set; } = 1;
    public PageSize PageSize { get; set; } = PageSize.p25;
}
