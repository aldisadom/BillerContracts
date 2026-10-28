using BillerContracts.Enums;

namespace BillerContracts.Responses.Series;

public record SeriesListResponse
{
    public List<SeriesResponse> Series { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
