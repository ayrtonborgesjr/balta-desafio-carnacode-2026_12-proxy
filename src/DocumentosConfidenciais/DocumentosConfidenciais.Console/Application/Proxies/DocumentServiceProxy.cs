using DocumentosConfidenciais.Console.Application.Services;
using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Interfaces;
using DocumentosConfidenciais.Console.Infrastructure;

namespace DocumentosConfidenciais.Console.Application.Proxies;

public class DocumentServiceProxy : IDocumentService
{
    private readonly DocumentRepository _repository;
    private RealDocumentService? _realService;

    private readonly Dictionary<string, ConfidentialDocument> _cache = new();
    private readonly List<string> _auditLog = new();

    public DocumentServiceProxy(DocumentRepository repository)
    {
        _repository = repository;
    }

    private RealDocumentService RealService
    {
        get
        {
            if (_realService == null)
            {
                System.Console.WriteLine("[Proxy] Inicializando serviço real sob demanda...");
                _realService = new RealDocumentService(_repository);
            }

            return _realService;
        }
    }

    public ConfidentialDocument? ViewDocument(string documentId, User user)
    {
        Log($"{user.Username} tentou visualizar {documentId}");

        if (_cache.TryGetValue(documentId, out var cachedDoc))
        {
            System.Console.WriteLine("[Proxy] Documento retornado do cache");
            return Authorize(user, cachedDoc);
        }

        var document = RealService.ViewDocument(documentId, user);

        if (document == null)
        {
            System.Console.WriteLine("❌ Documento não encontrado");
            return null;
        }

        _cache[documentId] = document;

        return Authorize(user, document);
    }

    public void EditDocument(string documentId, User user, string newContent)
    {
        Log($"{user.Username} tentou editar {documentId}");

        var document = RealService.ViewDocument(documentId, user);

        if (document == null)
        {
            System.Console.WriteLine("❌ Documento não encontrado");
            return;
        }

        if (!user.HasPermissionFor(document))
        {
            System.Console.WriteLine("❌ Acesso negado");
            Log($"ACESSO NEGADO para {user.Username}");
            return;
        }

        RealService.EditDocument(documentId, user, newContent);

        _cache.Remove(documentId); // invalida cache

        System.Console.WriteLine("✅ Documento atualizado com sucesso");
    }

    private ConfidentialDocument? Authorize(User user, ConfidentialDocument document)
    {
        if (!user.HasPermissionFor(document))
        {
            System.Console.WriteLine("❌ Acesso negado");
            Log($"ACESSO NEGADO para {user.Username}");
            return null;
        }

        System.Console.WriteLine($"✅ Acesso autorizado ao documento: {document.Title}");
        return document;
    }

    private void Log(string message)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
        _auditLog.Add(entry);
        System.Console.WriteLine($"[Audit] {entry}");
    }

    public void ShowAuditLog()
    {
        System.Console.WriteLine("\n=== Log de Auditoria ===");
        foreach (var entry in _auditLog)
            System.Console.WriteLine(entry);
    }
}