using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Programs;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Enums;
namespace AcademyERP.Domain.Entities.Admissions;

public class AdmissionApplication : BaseEntity
{
    // Program selected by the applicant
    public Guid ProgramId { get; set; }

    public Program Program { get; set; } = null!;


    // Course may be assigned later by admissions
    public Guid? CourseId { get; set; }

    public Course? Course { get; set; }


    // Student information
    public string StudentName { get; set; } = string.Empty;

    public int Age { get; set; }

    public Gender Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string? City { get; set; }


    // Contact information
    public string WhatsAppNumber { get; set; } = string.Empty;

    public string? Email { get; set; }


    // Learning preferences
    public string? PreferredTiming { get; set; }
    public bool IsConverted { get; set; }

    public Guid? StudentId { get; set; }

    public string? Message { get; set; }


    // Admission workflow
    public AdmissionStatus Status { get; set; } = AdmissionStatus.New;
}