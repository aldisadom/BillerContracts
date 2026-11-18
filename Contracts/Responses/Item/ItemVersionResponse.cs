namespace BillerContracts.Responses.Item;

public record ItemVersionResponse
{
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ChangedDate { get; set; }
}
