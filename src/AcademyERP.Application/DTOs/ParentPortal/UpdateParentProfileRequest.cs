namespace AcademyERP.Application.DTOs.ParentPortal;

public class UpdateParentProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
