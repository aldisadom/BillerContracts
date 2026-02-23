namespace BillerContracts.Responses.Item;

public record ItemListResponse
{
    public List<ItemResponse> Items { get; set; } = [];
}
