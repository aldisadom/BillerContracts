namespace BillerContracts.Responses;

public record FileDownloadResponse
{
    public Stream Content { get; set; } = default!;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
