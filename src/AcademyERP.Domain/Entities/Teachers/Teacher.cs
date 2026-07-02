using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.TeacherCourses;
using AcademyERP.Domain.Entities.TeacherAvailabilities;

namespace AcademyERP.Domain.Entities.Teachers;

public class Teacher : BaseEntity
{
    public Guid ApplicationUserId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
    public ICollection<TeacherAvailability> TeacherAvailabilities { get; set; } = new List<TeacherAvailability>();
}