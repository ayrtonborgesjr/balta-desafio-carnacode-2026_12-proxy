using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Enums;

namespace DocumentosConfidenciais.Console.Infrastructure;

public class DocumentRepository
{
    private readonly Dictionary<string, ConfidentialDocument> _database;

    public DocumentRepository()
    {
        System.Console.WriteLine("[Repository] Inicializando conexão com banco...");
        Thread.Sleep(1000); // Simula conexão pesada

        _database = new Dictionary<string, ConfidentialDocument>
        {
            ["DOC001"] = new ConfidentialDocument(
                "DOC001",
                "Relatório Financeiro Q4",
                "Conteúdo confidencial do relatório financeiro...",
                ClearanceLevel.Confidential
            ),

            ["DOC002"] = new ConfidentialDocument(
                "DOC002",
                "Estratégia de Mercado 2025",
                "Planos estratégicos altamente confidenciais...",
                ClearanceLevel.TopSecret
            ),

            ["DOC003"] = new ConfidentialDocument(
                "DOC003",
                "Manual do Funcionário",
                "Políticas e procedimentos internos...",
                ClearanceLevel.Public
            )
        };
    }

    public ConfidentialDocument? GetDocument(string documentId)
    {
        System.Console.WriteLine($"[Repository] Buscando documento {documentId}...");
        Thread.Sleep(500); // Simula operação custosa

        return _database.TryGetValue(documentId, out var document)
            ? document
            : null;
    }

    public void UpdateDocument(string documentId, string newContent)
    {
        System.Console.WriteLine($"[Repository] Atualizando documento {documentId}...");
        Thread.Sleep(300);

        if (_database.TryGetValue(documentId, out var document))
        {
            document.UpdateContent(newContent);
        }
    }
}