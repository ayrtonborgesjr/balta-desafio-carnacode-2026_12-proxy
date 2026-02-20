using DocumentosConfidenciais.Console.Application.Proxies;
using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Enums;
using DocumentosConfidenciais.Console.Domain.Interfaces;
using DocumentosConfidenciais.Console.Infrastructure;

Console.WriteLine("=== Sistema de Documentos Confidenciais (Proxy Pattern) ===\n");

// Infraestrutura
var repository = new DocumentRepository();

// Proxy (cliente só conhece a interface)
IDocumentService documentService = new DocumentServiceProxy(repository);

// Usuários
var manager = new User("joao.silva", ClearanceLevel.TopSecret);
var employee = new User("maria.santos", ClearanceLevel.Internal);

Console.WriteLine("\n--- 1️⃣ Gerente acessando documento TopSecret ---");
documentService.ViewDocument("DOC002", manager);

Console.WriteLine("\n--- 2️⃣ Funcionário tentando acessar mesmo documento ---");
documentService.ViewDocument("DOC002", employee);

Console.WriteLine("\n--- 3️⃣ Gerente acessando novamente (deve usar cache) ---");
documentService.ViewDocument("DOC002", manager);

Console.WriteLine("\n--- 4️⃣ Funcionário acessando documento permitido ---");
documentService.ViewDocument("DOC003", employee);

Console.WriteLine("\n--- 5️⃣ Gerente editando documento ---");
documentService.EditDocument("DOC003", manager, "Novo conteúdo atualizado...");

Console.WriteLine("\n--- 6️⃣ Gerente acessando documento atualizado ---");
documentService.ViewDocument("DOC003", manager);

// Exibir auditoria (cast apenas para demonstração)
if (documentService is DocumentServiceProxy proxy)
{
    proxy.ShowAuditLog();
}

Console.WriteLine("\n=== Demonstração concluída ===");
