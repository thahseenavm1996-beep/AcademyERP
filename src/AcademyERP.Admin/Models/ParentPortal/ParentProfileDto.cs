namespace AcademyERP.Admin.Models.ParentPortal;

public class ParentProfileDto
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }

    public string ParentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
