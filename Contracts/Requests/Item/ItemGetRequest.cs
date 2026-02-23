namespace BillerContracts.Requests.Item;

public record ItemGetRequest
{
    public Guid? CustomerId { get; set; }
}
