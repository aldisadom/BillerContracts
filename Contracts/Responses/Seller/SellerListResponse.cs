using BillerContracts.Enums;

namespace BillerContracts.Responses.Seller;

public record SellerListResponse
{
    public List<SellerResponse> Sellers { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
