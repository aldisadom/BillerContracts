namespace BillerContracts.Clients;

/// <summary>
/// Full base URL of each Biller API resource, e.g. <c>https://biller.example.com/users</c>.
/// </summary>
public class ClientsOptions
{
    public string UserUrl { get; set; } = string.Empty;
    public string CustomerUrl { get; set; } = string.Empty;
    public string SellerUrl { get; set; } = string.Empty;
    public string ItemUrl { get; set; } = string.Empty;
    public string InvoiceUrl { get; set; } = string.Empty;
    public string SeriesUrl { get; set; } = string.Empty;
}
