using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.StudentParents;
using AcademyERP.Domain.Entities.ClassReports;

namespace AcademyERP.Domain.Entities.Students;

public class Student : BaseEntity
{
    // Link to the login account (ASP.NET Core Identity)
    public Guid? ApplicationUserId { get; set; }

    // Academy Student Information
    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    // Internal notes
    public string? Remarks { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<ClassReport> ClassReports { get; set; } = new List<ClassReport>();

    public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
}