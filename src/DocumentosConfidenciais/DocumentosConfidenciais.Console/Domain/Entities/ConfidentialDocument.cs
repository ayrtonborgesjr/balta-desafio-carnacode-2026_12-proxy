using DocumentosConfidenciais.Console.Domain.Enums;

namespace DocumentosConfidenciais.Console.Domain.Entities;

public class ConfidentialDocument
{
    public string Id { get; }
    public string Title { get; }
    public ClearanceLevel RequiredClearance { get; }
    public long SizeInBytes => Content.Length * sizeof(char);

    private string _content;
    public string Content => _content;

    public ConfidentialDocument(
        string id,
        string title,
        string content,
        ClearanceLevel requiredClearance)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        _content = content ?? throw new ArgumentNullException(nameof(content));
        RequiredClearance = requiredClearance;
    }

    public void UpdateContent(string newContent)
    {
        _content = newContent ?? throw new ArgumentNullException(nameof(newContent));
    }
}