namespace AcademyERP.Application.DTOs.Parents;

public class ParentResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public string Status { get; set; } = string.Empty;

    public int ChildrenCount { get; set; }

    public List<ParentStudentResponse> Children { get; set; } = new();
}

public class ParentStudentResponse
{
    public Guid StudentId { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}