using DocumentosConfidenciais.Console.Domain.Entities;
using DocumentosConfidenciais.Console.Domain.Enums;

namespace DocumentosConfidenciais.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateUser_WithValidParameters()
    {
        // Arrange
        var username = "john.doe";
        var clearanceLevel = ClearanceLevel.Confidential;

        // Act
        var user = new User(username, clearanceLevel);

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(clearanceLevel, user.ClearanceLevel);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUsernameIsNull()
    {
        // Arrange
        string? username = null;
        var clearanceLevel = ClearanceLevel.Public;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new User(username!, clearanceLevel));
    }

    [Theory]
    [InlineData(ClearanceLevel.TopSecret, ClearanceLevel.Public, true)]
    [InlineData(ClearanceLevel.TopSecret, ClearanceLevel.Confidential, true)]
    [InlineData(ClearanceLevel.TopSecret, ClearanceLevel.TopSecret, true)]
    [InlineData(ClearanceLevel.Confidential, ClearanceLevel.TopSecret, false)]
    [InlineData(ClearanceLevel.Public, ClearanceLevel.Internal, false)]
    [InlineData(ClearanceLevel.Internal, ClearanceLevel.Internal, true)]
    public void HasPermissionFor_ShouldReturnCorrectPermission_BasedOnClearanceLevel(
        ClearanceLevel userLevel,
        ClearanceLevel documentLevel,
        bool expectedResult)
    {
        // Arrange
        var user = new User("test.user", userLevel);
        var document = new ConfidentialDocument(
            "DOC001",
            "Test Document",
            "Content",
            documentLevel);

        // Act
        var result = user.HasPermissionFor(document);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void HasPermissionFor_ShouldReturnTrue_WhenUserHasExactSameClearanceLevel()
    {
        // Arrange
        var user = new User("test.user", ClearanceLevel.Restricted);
        var document = new ConfidentialDocument(
            "DOC001",
            "Restricted Document",
            "Sensitive content",
            ClearanceLevel.Restricted);

        // Act
        var result = user.HasPermissionFor(document);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPermissionFor_ShouldReturnFalse_WhenUserHasLowerClearanceLevel()
    {
        // Arrange
        var user = new User("low.level", ClearanceLevel.Public);
        var document = new ConfidentialDocument(
            "DOC002",
            "Top Secret Document",
            "Highly classified",
            ClearanceLevel.TopSecret);

        // Act
        var result = user.HasPermissionFor(document);

        // Assert
        Assert.False(result);
    }
}

