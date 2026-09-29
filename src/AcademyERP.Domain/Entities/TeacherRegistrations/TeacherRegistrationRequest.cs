using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.TeacherRegistrations;

public class TeacherRegistrationRequest : BaseEntity
{
    // Personal Information
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Address { get; set; }

    // Professional Information
    public string Qualification { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public string? TeachingExperience { get; set; }
    public string? Remarks { get; set; }

    // Registration workflow
    public TeacherRegistrationStatus Status { get; set; } = TeacherRegistrationStatus.Pending;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewRemarks { get; set; }

    public ICollection<TeacherRegistrationAvailability> Availability { get; set; }
        = new List<TeacherRegistrationAvailability>();

    public ICollection<TeacherRegistrationClass> CurrentClasses { get; set; }
        = new List<TeacherRegistrationClass>();
}
