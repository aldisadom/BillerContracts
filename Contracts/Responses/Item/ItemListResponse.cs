using BillerContracts.Enums;

namespace BillerContracts.Responses.Item;

public record ItemListResponse
{
    public List<ItemResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
