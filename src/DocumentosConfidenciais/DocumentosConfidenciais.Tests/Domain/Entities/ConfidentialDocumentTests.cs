using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Enums;

namespace DocumentosConfidenciais.Tests.Domain.Entities;

public class ConfidentialDocumentTests
{
    [Fact]
    public void Constructor_ShouldCreateDocument_WithValidParameters()
    {
        // Arrange
        var id = "DOC001";
        var title = "Test Document";
        var content = "This is test content";
        var clearanceLevel = ClearanceLevel.Confidential;

        // Act
        var document = new ConfidentialDocument(id, title, content, clearanceLevel);

        // Assert
        Assert.Equal(id, document.Id);
        Assert.Equal(title, document.Title);
        Assert.Equal(content, document.Content);
        Assert.Equal(clearanceLevel, document.RequiredClearance);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenIdIsNull()
    {
        // Arrange
        string? id = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ConfidentialDocument(id!, "Title", "Content", ClearanceLevel.Public));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenTitleIsNull()
    {
        // Arrange
        string? title = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ConfidentialDocument("DOC001", title!, "Content", ClearanceLevel.Public));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenContentIsNull()
    {
        // Arrange
        string? content = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ConfidentialDocument("DOC001", "Title", content!, ClearanceLevel.Public));
    }

    [Fact]
    public void UpdateContent_ShouldUpdateDocumentContent_WithValidContent()
    {
        // Arrange
        var document = new ConfidentialDocument(
            "DOC001",
            "Test Document",
            "Original content",
            ClearanceLevel.Internal);
        var newContent = "Updated content";

        // Act
        document.UpdateContent(newContent);

        // Assert
        Assert.Equal(newContent, document.Content);
    }

    [Fact]
    public void UpdateContent_ShouldThrowArgumentNullException_WhenContentIsNull()
    {
        // Arrange
        var document = new ConfidentialDocument(
            "DOC001",
            "Test Document",
            "Original content",
            ClearanceLevel.Internal);
        string? newContent = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => document.UpdateContent(newContent!));
    }

    [Fact]
    public void SizeInBytes_ShouldCalculateCorrectSize()
    {
        // Arrange
        var content = "Test";
        var document = new ConfidentialDocument(
            "DOC001",
            "Title",
            content,
            ClearanceLevel.Public);
        var expectedSize = content.Length * sizeof(char);

        // Act
        var actualSize = document.SizeInBytes;

        // Assert
        Assert.Equal(expectedSize, actualSize);
    }

    [Fact]
    public void SizeInBytes_ShouldUpdateAfterContentChange()
    {
        // Arrange
        var document = new ConfidentialDocument(
            "DOC001",
            "Title",
            "Short",
            ClearanceLevel.Public);
        var newContent = "This is a much longer content";
        var expectedSize = newContent.Length * sizeof(char);

        // Act
        document.UpdateContent(newContent);

        // Assert
        Assert.Equal(expectedSize, document.SizeInBytes);
    }

    [Theory]
    [InlineData(ClearanceLevel.Public)]
    [InlineData(ClearanceLevel.Internal)]
    [InlineData(ClearanceLevel.Confidential)]
    [InlineData(ClearanceLevel.Restricted)]
    [InlineData(ClearanceLevel.TopSecret)]
    public void Constructor_ShouldAcceptAllClearanceLevels(ClearanceLevel level)
    {
        // Arrange & Act
        var document = new ConfidentialDocument(
            "DOC001",
            "Test",
            "Content",
            level);

        // Assert
        Assert.Equal(level, document.RequiredClearance);
    }
}

