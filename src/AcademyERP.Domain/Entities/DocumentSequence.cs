namespace AcademyERP.Domain.Entities;

public class DocumentSequence
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Prefix { get; set; } = string.Empty;

    public int NextNumber { get; set; }
}