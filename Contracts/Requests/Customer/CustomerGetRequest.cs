namespace BillerContracts.Requests.Customer;

public record CustomerGetRequest
{
    public Guid? SellerId { get; set; }
}
