namespace BillerContracts.Responses.Item;

public record ItemVersionListResponse
{
    public List<ItemVersionResponse> Versions { get; set; } = [];
}
