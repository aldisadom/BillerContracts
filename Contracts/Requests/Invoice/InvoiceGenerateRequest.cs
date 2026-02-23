using BillerContracts.Enums;

namespace BillerContracts.Requests.Invoice;

public record InvoiceGenerateRequest
{
    public Guid Id { get; set; }
    public Language LanguageCode { get; set; } = Language.LT;
    public DocumentType DocumentType { get; set; } = DocumentType.Invoice;
}
