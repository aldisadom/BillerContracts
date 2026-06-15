using BillerContracts.Enums;

namespace BillerContracts.Requests.Seller;

public record UserGetRequest
{
    public int Page { get; set; } = 1;
    public PageSize PageSize { get; set; } = PageSize.p25;
}
