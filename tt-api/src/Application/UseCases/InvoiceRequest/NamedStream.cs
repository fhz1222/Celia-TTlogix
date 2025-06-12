namespace Application.UseCases.InvoiceRequest;

public class NamedStream
{
    public MemoryStream Content { get; set; } = default!;
    public string FileName { get; set; } = default!;

    public NamedStream Clone() => new()
    {
        FileName = FileName,
        Content = new MemoryStream(Content.ToArray())
    };
}