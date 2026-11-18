namespace BillerContracts.Responses.Item;

public record ItemVersionResponse
{
    public Guid ItemId { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ChangedDate { get; set; }
}
