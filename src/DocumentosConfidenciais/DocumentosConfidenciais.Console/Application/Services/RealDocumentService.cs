using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Interfaces;
using DocumentosConfidenciais.Console.Infrastructure;

namespace DocumentosConfidenciais.Console.Application.Services;

public class RealDocumentService : IDocumentService
{
    private readonly DocumentRepository _repository;

    public RealDocumentService(DocumentRepository repository)
    {
        _repository = repository;
    }

    public ConfidentialDocument? ViewDocument(string documentId, User user)
    {
        return _repository.GetDocument(documentId);
    }

    public void EditDocument(string documentId, User user, string newContent)
    {
        _repository.UpdateDocument(documentId, newContent);
    }
}