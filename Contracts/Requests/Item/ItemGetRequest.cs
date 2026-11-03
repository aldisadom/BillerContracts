namespace BillerContracts.Requests.Item;

public record ItemGetRequest : PagedRequest
{
    public Guid? CustomerId { get; set; }
}
