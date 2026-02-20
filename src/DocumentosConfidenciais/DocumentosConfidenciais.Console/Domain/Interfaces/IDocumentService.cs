using DocumentosConfidenciais.Console.Domain.Entities;

namespace DocumentosConfidenciais.Console.Domain.Interfaces;

public interface IDocumentService
{
    ConfidentialDocument? ViewDocument(string documentId, User user);
    void EditDocument(string documentId, User user, string newContent);
}