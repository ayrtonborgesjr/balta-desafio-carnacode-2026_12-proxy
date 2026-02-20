using DocumentosConfidenciais.Console.Domain.Enums;

namespace DocumentosConfidenciais.Console.Domain.Entities;

public class User
{
    public string Username { get; }
    public ClearanceLevel ClearanceLevel { get; }

    public User(string username, ClearanceLevel clearanceLevel)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        ClearanceLevel = clearanceLevel;
    }

    public bool HasPermissionFor(ConfidentialDocument document)
    {
        return ClearanceLevel >= document.RequiredClearance;
    }
}