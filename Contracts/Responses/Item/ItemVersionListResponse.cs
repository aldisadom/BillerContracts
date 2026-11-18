namespace BillerContracts.Responses.Item;

public record ItemVersionListResponse
{
    public Guid ItemId { get; set; }
    public List<ItemVersionResponse> Versions { get; set; } = [];
}
