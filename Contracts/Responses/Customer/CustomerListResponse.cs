namespace BillerContracts.Responses.Customer;

public record CustomerListResponse : PagedResponse
{
    public List<CustomerResponse> Customers { get; set; } = [];
}
