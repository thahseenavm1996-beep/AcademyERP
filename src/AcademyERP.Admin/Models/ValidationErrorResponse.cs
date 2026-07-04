namespace AcademyERP.Admin.Models;

public class ValidationErrorResponse
{
    public string? Type { get; set; }

    public string? Title { get; set; }

    public int Status { get; set; }

    public Dictionary<string, string[]> Errors { get; set; } = new();

    public string? TraceId { get; set; }
}