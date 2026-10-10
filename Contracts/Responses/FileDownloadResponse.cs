namespace BillerContracts.Clients;

public class FileDownloadResult
{
    public Stream Content { get; set; } = default!;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
