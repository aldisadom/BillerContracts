using BillerContracts.Enums;

namespace BillerContracts.Responses;

public abstract record PagedResponse
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
