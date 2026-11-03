namespace BillerContracts.Responses.Series;

public record SeriesListResponse : PagedResponse
{
    public List<SeriesResponse> Series { get; set; } = [];
}
