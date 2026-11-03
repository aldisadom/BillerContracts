namespace BillerContracts.Requests.Series;

public record SeriesGetRequest : PagedRequest
{
    public Guid? SellerId { get; set; }
}
