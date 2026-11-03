namespace BillerContracts.Responses.Item;

public record ItemListResponse : PagedResponse
{
    public List<ItemResponse> Items { get; set; } = [];
}
