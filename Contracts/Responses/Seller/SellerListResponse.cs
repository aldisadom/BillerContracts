namespace BillerContracts.Responses.Seller;

public record SellerListResponse : PagedResponse
{
    public List<SellerResponse> Sellers { get; set; } = [];
}
