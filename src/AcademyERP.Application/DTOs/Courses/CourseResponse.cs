namespace AcademyERP.Application.DTOs.Courses;

public class CourseResponse
{
    public Guid Id { get; set; }

    public Guid ProgramId { get; set; }

    public string ProgramName { get; set; } = string.Empty;

    public string CourseCode { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal StandardMonthlyFee { get; set; }

    public bool IsGroupClassAllowed { get; set; }

    public bool IsActive { get; set; }
}