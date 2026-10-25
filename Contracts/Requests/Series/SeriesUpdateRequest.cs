namespace BillerContracts.Requests.Series;

public record SeriesUpdateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NextNumber { get; set; }
}
