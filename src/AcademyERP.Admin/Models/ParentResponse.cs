namespace AcademyERP.Admin.Models;

public class ParentResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public string Status { get; set; } = string.Empty;
}