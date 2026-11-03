namespace BillerContracts.Requests.Seller;

public record SellerGetRequest : PagedRequest
{
    public Guid? UserId { get; set; }
}
