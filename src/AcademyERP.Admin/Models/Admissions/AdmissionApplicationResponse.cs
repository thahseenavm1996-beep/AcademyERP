namespace AcademyERP.Admin.Models.Admissions;

public class AdmissionApplicationResponse
{
    public Guid Id { get; set; }

    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }



    public string StudentName { get; set; } = string.Empty;

    public int Age { get; set; }

    public int Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string? City { get; set; }

    public string WhatsAppNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PreferredTiming { get; set; }

    public string? Message { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }
}