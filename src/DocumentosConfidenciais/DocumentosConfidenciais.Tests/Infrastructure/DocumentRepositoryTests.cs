using DocumentosConfidenciais.Console.Domain.Enums;
using DocumentosConfidenciais.Console.Infrastructure;

namespace DocumentosConfidenciais.Tests.Infrastructure;

public class DocumentRepositoryTests
{
    [Fact]
    public void Constructor_ShouldInitializeRepository_WithPredefinedDocuments()
    {
        // Act
        var repository = new DocumentRepository();

        // Assert
        var doc1 = repository.GetDocument("DOC001");
        var doc2 = repository.GetDocument("DOC002");
        var doc3 = repository.GetDocument("DOC003");

        Assert.NotNull(doc1);
        Assert.NotNull(doc2);
        Assert.NotNull(doc3);
    }

    [Fact]
    public void GetDocument_ShouldReturnDocument_WhenDocumentExists()
    {
        // Arrange
        var repository = new DocumentRepository();
        var documentId = "DOC001";

        // Act
        var document = repository.GetDocument(documentId);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(documentId, document.Id);
        Assert.Equal("Relatório Financeiro Q4", document.Title);
        Assert.Equal(ClearanceLevel.Confidential, document.RequiredClearance);
    }

    [Fact]
    public void GetDocument_ShouldReturnNull_WhenDocumentDoesNotExist()
    {
        // Arrange
        var repository = new DocumentRepository();
        var nonExistentId = "DOC999";

        // Act
        var document = repository.GetDocument(nonExistentId);

        // Assert
        Assert.Null(document);
    }

    [Theory]
    [InlineData("DOC001", "Relatório Financeiro Q4", ClearanceLevel.Confidential)]
    [InlineData("DOC002", "Estratégia de Mercado 2025", ClearanceLevel.TopSecret)]
    [InlineData("DOC003", "Manual do Funcionário", ClearanceLevel.Public)]
    public void GetDocument_ShouldReturnCorrectDocument_ForEachPredefinedDocument(
        string documentId,
        string expectedTitle,
        ClearanceLevel expectedLevel)
    {
        // Arrange
        var repository = new DocumentRepository();

        // Act
        var document = repository.GetDocument(documentId);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(documentId, document.Id);
        Assert.Equal(expectedTitle, document.Title);
        Assert.Equal(expectedLevel, document.RequiredClearance);
    }

    [Fact]
    public void UpdateDocument_ShouldUpdateContent_WhenDocumentExists()
    {
        // Arrange
        var repository = new DocumentRepository();
        var documentId = "DOC001";
        var newContent = "Updated content for testing";
        
        // Get original document
        var originalDocument = repository.GetDocument(documentId);
        var originalContent = originalDocument!.Content;

        // Act
        repository.UpdateDocument(documentId, newContent);

        // Assert
        var updatedDocument = repository.GetDocument(documentId);
        Assert.NotNull(updatedDocument);
        Assert.Equal(newContent, updatedDocument.Content);
        Assert.NotEqual(originalContent, updatedDocument.Content);
    }

    [Fact]
    public void UpdateDocument_ShouldNotThrowException_WhenDocumentDoesNotExist()
    {
        // Arrange
        var repository = new DocumentRepository();
        var nonExistentId = "DOC999";
        var newContent = "New content";

        // Act & Assert
        var exception = Record.Exception(() => repository.UpdateDocument(nonExistentId, newContent));
        Assert.Null(exception);
    }

    [Fact]
    public void GetDocument_ShouldReturnSameInstance_OnMultipleCalls()
    {
        // Arrange
        var repository = new DocumentRepository();
        var documentId = "DOC001";

        // Act
        var document1 = repository.GetDocument(documentId);
        var document2 = repository.GetDocument(documentId);

        // Assert
        Assert.Same(document1, document2);
    }

    [Fact]
    public void UpdateDocument_ShouldPersistChanges_AcrossMultipleRetrievals()
    {
        // Arrange
        var repository = new DocumentRepository();
        var documentId = "DOC002";
        var newContent = "Persistent updated content";

        // Act
        repository.UpdateDocument(documentId, newContent);
        var document1 = repository.GetDocument(documentId);
        var document2 = repository.GetDocument(documentId);

        // Assert
        Assert.NotNull(document1);
        Assert.NotNull(document2);
        Assert.Equal(newContent, document1.Content);
        Assert.Equal(newContent, document2.Content);
    }
}

