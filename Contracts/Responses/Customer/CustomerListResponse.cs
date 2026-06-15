using BillerContracts.Enums;

namespace BillerContracts.Responses.Customer;

public record CustomerListResponse
{
    public List<CustomerResponse> Customers { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public PageSize PageSize { get; set; }
}
