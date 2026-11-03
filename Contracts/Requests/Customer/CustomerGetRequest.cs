namespace BillerContracts.Requests.Customer;

public record CustomerGetRequest : PagedRequest
{
    public Guid? SellerId { get; set; }
}
