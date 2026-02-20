using DocumentosConfidenciais.Console.Application.Proxies;
using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Enums;
using DocumentosConfidenciais.Console.Infrastructure;

namespace DocumentosConfidenciais.Tests.Application.Proxies;

public class DocumentServiceProxyTests
{
    [Fact]
    public void Constructor_ShouldCreateProxy_WithValidRepository()
    {
        // Arrange
        var repository = new DocumentRepository();

        // Act
        var proxy = new DocumentServiceProxy(repository);

        // Assert
        Assert.NotNull(proxy);
    }

    [Fact]
    public void ViewDocument_ShouldReturnDocument_WhenUserHasPermission()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("manager", ClearanceLevel.TopSecret);
        var documentId = "DOC002";

        // Act
        var document = proxy.ViewDocument(documentId, user);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(documentId, document.Id);
        Assert.Equal("Estratégia de Mercado 2025", document.Title);
    }

    [Fact]
    public void ViewDocument_ShouldReturnNull_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var lowLevelUser = new User("employee", ClearanceLevel.Public);
        var topSecretDocId = "DOC002"; // TopSecret document

        // Act
        var document = proxy.ViewDocument(topSecretDocId, lowLevelUser);

        // Assert
        Assert.Null(document);
    }

    [Fact]
    public void ViewDocument_ShouldReturnNull_WhenDocumentDoesNotExist()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("user", ClearanceLevel.TopSecret);
        var nonExistentId = "DOC999";

        // Act
        var document = proxy.ViewDocument(nonExistentId, user);

        // Assert
        Assert.Null(document);
    }

    [Theory]
    [InlineData(ClearanceLevel.TopSecret, "DOC001", true)]  // Confidential doc
    [InlineData(ClearanceLevel.TopSecret, "DOC002", true)]  // TopSecret doc
    [InlineData(ClearanceLevel.TopSecret, "DOC003", true)]  // Public doc
    [InlineData(ClearanceLevel.Confidential, "DOC001", true)]  // Confidential doc
    [InlineData(ClearanceLevel.Confidential, "DOC002", false)] // TopSecret doc - denied
    [InlineData(ClearanceLevel.Public, "DOC001", false)] // Confidential doc - denied
    [InlineData(ClearanceLevel.Public, "DOC002", false)] // TopSecret doc - denied
    [InlineData(ClearanceLevel.Public, "DOC003", true)]  // Public doc
    public void ViewDocument_ShouldEnforceAccessControl_BasedOnClearanceLevel(
        ClearanceLevel userLevel,
        string documentId,
        bool shouldHaveAccess)
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("test.user", userLevel);

        // Act
        var document = proxy.ViewDocument(documentId, user);

        // Assert
        if (shouldHaveAccess)
        {
            Assert.NotNull(document);
            Assert.Equal(documentId, document.Id);
        }
        else
        {
            Assert.Null(document);
        }
    }

    [Fact]
    public void ViewDocument_ShouldUseCache_OnSecondCall()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("manager", ClearanceLevel.TopSecret);
        var documentId = "DOC001";

        // Act - First call loads from repository
        var document1 = proxy.ViewDocument(documentId, user);
        
        // Act - Second call should return cached version (same instance)
        var document2 = proxy.ViewDocument(documentId, user);

        // Assert - Both should be the same cached instance
        Assert.NotNull(document1);
        Assert.NotNull(document2);
        Assert.Same(document1, document2);
        
        // Note: Since cache stores reference, if repository modifies the object,
        // the cache will reflect it too (both point to same object)
        repository.UpdateDocument(documentId, "Modified content");
        Assert.Equal("Modified content", document2.Content); // Proves it's the same reference
    }

    [Fact]
    public void EditDocument_ShouldUpdateDocument_WhenUserHasPermission()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("manager", ClearanceLevel.TopSecret);
        var documentId = "DOC001";
        var newContent = "Updated content via proxy";

        // Act
        proxy.EditDocument(documentId, user, newContent);

        // Assert - Need to check via repository since cache is invalidated
        var updatedDoc = repository.GetDocument(documentId);
        Assert.NotNull(updatedDoc);
        Assert.Equal(newContent, updatedDoc.Content);
    }

    [Fact]
    public void EditDocument_ShouldNotUpdate_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var lowLevelUser = new User("employee", ClearanceLevel.Public);
        var topSecretDocId = "DOC002";
        
        var originalDoc = repository.GetDocument(topSecretDocId);
        var originalContent = originalDoc!.Content;
        var newContent = "Attempted unauthorized update";

        // Act
        proxy.EditDocument(topSecretDocId, lowLevelUser, newContent);

        // Assert - Content should remain unchanged
        var document = repository.GetDocument(topSecretDocId);
        Assert.NotNull(document);
        Assert.Equal(originalContent, document.Content);
        Assert.NotEqual(newContent, document.Content);
    }

    [Fact]
    public void EditDocument_ShouldNotThrow_WhenDocumentDoesNotExist()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("user", ClearanceLevel.TopSecret);
        var nonExistentId = "DOC999";
        var newContent = "New content";

        // Act & Assert
        var exception = Record.Exception(() => 
            proxy.EditDocument(nonExistentId, user, newContent));
        Assert.Null(exception);
    }

    [Fact]
    public void EditDocument_ShouldInvalidateCache()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("manager", ClearanceLevel.TopSecret);
        var documentId = "DOC001";
        
        // Load document into cache
        var cachedDoc = proxy.ViewDocument(documentId, user);
        var cachedContent = cachedDoc!.Content;
        
        var newContent = "Updated content that invalidates cache";

        // Act
        proxy.EditDocument(documentId, user, newContent);
        
        // View again - should get fresh copy from repository
        var freshDoc = proxy.ViewDocument(documentId, user);

        // Assert
        Assert.NotNull(freshDoc);
        Assert.Equal(newContent, freshDoc.Content);
        Assert.NotEqual(cachedContent, freshDoc.Content);
    }

    [Theory]
    [InlineData(ClearanceLevel.TopSecret, "DOC002", true)]  // TopSecret can edit TopSecret
    [InlineData(ClearanceLevel.Confidential, "DOC001", true)]  // Confidential can edit Confidential
    [InlineData(ClearanceLevel.Confidential, "DOC002", false)] // Confidential cannot edit TopSecret
    [InlineData(ClearanceLevel.Public, "DOC003", true)]  // Public can edit Public
    [InlineData(ClearanceLevel.Public, "DOC001", false)] // Public cannot edit Confidential
    public void EditDocument_ShouldEnforceAccessControl_BasedOnClearanceLevel(
        ClearanceLevel userLevel,
        string documentId,
        bool shouldAllowEdit)
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("test.user", userLevel);
        
        var originalDoc = repository.GetDocument(documentId);
        var originalContent = originalDoc!.Content;
        var newContent = "Test update content";

        // Act
        proxy.EditDocument(documentId, user, newContent);

        // Assert
        var document = repository.GetDocument(documentId);
        Assert.NotNull(document);
        
        if (shouldAllowEdit)
        {
            Assert.Equal(newContent, document.Content);
        }
        else
        {
            Assert.Equal(originalContent, document.Content);
        }
    }

    [Fact]
    public void ViewDocument_ShouldInitializeRealService_OnFirstCall()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("user", ClearanceLevel.TopSecret);

        // Act - First call should initialize the real service
        var document = proxy.ViewDocument("DOC001", user);

        // Assert
        Assert.NotNull(document);
    }

    [Fact]
    public void ShowAuditLog_ShouldNotThrow()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user = new User("user", ClearanceLevel.TopSecret);

        // Perform some operations
        proxy.ViewDocument("DOC001", user);
        proxy.EditDocument("DOC001", user, "New content");

        // Act & Assert
        var exception = Record.Exception(() => proxy.ShowAuditLog());
        Assert.Null(exception);
    }

    [Fact]
    public void Proxy_ShouldMaintainCache_AcrossMultipleUsers()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var user1 = new User("user1", ClearanceLevel.TopSecret);
        var user2 = new User("user2", ClearanceLevel.TopSecret);
        var documentId = "DOC001";

        // Act
        var doc1 = proxy.ViewDocument(documentId, user1);
        var doc2 = proxy.ViewDocument(documentId, user2);

        // Assert - Should return same cached instance
        Assert.NotNull(doc1);
        Assert.NotNull(doc2);
        Assert.Same(doc1, doc2);
    }

    [Fact]
    public void ViewDocument_ShouldCacheOnlyAuthorizedDocuments()
    {
        // Arrange
        var repository = new DocumentRepository();
        var proxy = new DocumentServiceProxy(repository);
        var lowLevelUser = new User("employee", ClearanceLevel.Public);
        var topSecretDocId = "DOC002";

        // Act - Try to view document without permission
        var deniedDoc = proxy.ViewDocument(topSecretDocId, lowLevelUser);

        // Now try with authorized user
        var authorizedUser = new User("manager", ClearanceLevel.TopSecret);
        var allowedDoc = proxy.ViewDocument(topSecretDocId, authorizedUser);

        // Assert
        Assert.Null(deniedDoc); // Access denied
        Assert.NotNull(allowedDoc); // Access granted
    }
}

