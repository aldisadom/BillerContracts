using BillerContracts.Enums;

namespace BillerContracts.Requests.Seller;

public record SellerGetRequest
{
    public Guid? UserId { get; set; }
    public int Page { get; set; } = 1;
    public PageSize PageSize { get; set; } = PageSize.p25;
}
