namespace AcademyERP.Application.DTOs.Lookups;

public class ClassDurationResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Minutes { get; set; }
}