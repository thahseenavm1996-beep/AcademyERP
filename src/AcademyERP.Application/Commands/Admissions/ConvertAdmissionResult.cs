namespace AcademyERP.Application.Commands.Admissions;

public class ConvertAdmissionResult
{
    public Guid StudentId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? EnrollmentId { get; set; }

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}