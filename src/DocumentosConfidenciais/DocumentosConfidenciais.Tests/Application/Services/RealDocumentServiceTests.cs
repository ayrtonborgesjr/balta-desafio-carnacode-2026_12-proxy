using DocumentosConfidenciais.Console.Application.Services;
using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Enums;
using DocumentosConfidenciais.Console.Infrastructure;

namespace DocumentosConfidenciais.Tests.Application.Services;

public class RealDocumentServiceTests
{
    [Fact]
    public void Constructor_ShouldCreateService_WithValidRepository()
    {
        // Arrange
        var repository = new DocumentRepository();

        // Act
        var service = new RealDocumentService(repository);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ViewDocument_ShouldReturnDocument_WhenDocumentExists()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("test.user", ClearanceLevel.TopSecret);
        var documentId = "DOC001";

        // Act
        var document = service.ViewDocument(documentId, user);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(documentId, document.Id);
    }

    [Fact]
    public void ViewDocument_ShouldReturnNull_WhenDocumentDoesNotExist()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("test.user", ClearanceLevel.TopSecret);
        var nonExistentId = "DOC999";

        // Act
        var document = service.ViewDocument(nonExistentId, user);

        // Assert
        Assert.Null(document);
    }

    [Theory]
    [InlineData("DOC001")]
    [InlineData("DOC002")]
    [InlineData("DOC003")]
    public void ViewDocument_ShouldReturnDocument_ForAllPredefinedDocuments(string documentId)
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("test.user", ClearanceLevel.TopSecret);

        // Act
        var document = service.ViewDocument(documentId, user);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(documentId, document.Id);
    }

    [Fact]
    public void ViewDocument_ShouldNotCheckPermissions_ReturnsDocumentRegardlessOfUserLevel()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var lowLevelUser = new User("low.user", ClearanceLevel.Public);
        var topSecretDocId = "DOC002";

        // Act
        var document = service.ViewDocument(topSecretDocId, lowLevelUser);

        // Assert - RealDocumentService não verifica permissões, apenas retorna o documento
        Assert.NotNull(document);
        Assert.Equal(topSecretDocId, document.Id);
        Assert.Equal(ClearanceLevel.TopSecret, document.RequiredClearance);
    }

    [Fact]
    public void EditDocument_ShouldUpdateDocument_WhenDocumentExists()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("test.user", ClearanceLevel.TopSecret);
        var documentId = "DOC001";
        var newContent = "Updated content via service";

        // Get original content
        var originalDoc = service.ViewDocument(documentId, user);
        var originalContent = originalDoc!.Content;

        // Act
        service.EditDocument(documentId, user, newContent);

        // Assert
        var updatedDoc = service.ViewDocument(documentId, user);
        Assert.NotNull(updatedDoc);
        Assert.Equal(newContent, updatedDoc.Content);
        Assert.NotEqual(originalContent, updatedDoc.Content);
    }

    [Fact]
    public void EditDocument_ShouldNotThrowException_WhenDocumentDoesNotExist()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("test.user", ClearanceLevel.TopSecret);
        var nonExistentId = "DOC999";
        var newContent = "New content";

        // Act & Assert
        var exception = Record.Exception(() => 
            service.EditDocument(nonExistentId, user, newContent));
        Assert.Null(exception);
    }

    [Fact]
    public void EditDocument_ShouldPersistChanges()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("editor", ClearanceLevel.TopSecret);
        var documentId = "DOC003";
        var newContent = "Persistent changes test";

        // Act
        service.EditDocument(documentId, user, newContent);
        var doc1 = service.ViewDocument(documentId, user);
        var doc2 = service.ViewDocument(documentId, user);

        // Assert
        Assert.NotNull(doc1);
        Assert.NotNull(doc2);
        Assert.Equal(newContent, doc1.Content);
        Assert.Equal(newContent, doc2.Content);
    }

    [Fact]
    public void EditDocument_ShouldNotCheckPermissions_AllowsAnyUserToEdit()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var lowLevelUser = new User("low.user", ClearanceLevel.Public);
        var topSecretDocId = "DOC002";
        var newContent = "Modified by low level user";

        // Act - RealDocumentService não verifica permissões
        var exception = Record.Exception(() => 
            service.EditDocument(topSecretDocId, lowLevelUser, newContent));

        // Assert
        Assert.Null(exception);
        var document = service.ViewDocument(topSecretDocId, lowLevelUser);
        Assert.Equal(newContent, document!.Content);
    }

    [Fact]
    public void ViewDocument_ShouldReturnSameDocumentInstance_OnMultipleCalls()
    {
        // Arrange
        var repository = new DocumentRepository();
        var service = new RealDocumentService(repository);
        var user = new User("test.user", ClearanceLevel.TopSecret);
        var documentId = "DOC001";

        // Act
        var doc1 = service.ViewDocument(documentId, user);
        var doc2 = service.ViewDocument(documentId, user);

        // Assert
        Assert.Same(doc1, doc2);
    }
}

