namespace BillerContracts.Requests.Series;

public record SeriesAddRequest
{
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
}
