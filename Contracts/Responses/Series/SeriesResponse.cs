namespace BillerContracts.Responses.Series;

public record SeriesResponse
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NextNumber { get; set; }
}
