using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.TeacherStudentRequests;

public class TeacherStudentRequest : BaseEntity
{
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }

    // Always store the information supplied by the teacher.
    // This is especially important when the student is new.
    public string StudentName { get; set; } = string.Empty;
    public string StudentPhoneNumber { get; set; } = string.Empty;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public TeacherStudentRequestStatus Status { get; set; }
        = TeacherStudentRequestStatus.Pending;

    public decimal? StudentFee { get; set; }
    public decimal? TeacherSalary { get; set; }

    public string? TeacherRemarks { get; set; }
    public string? AdminRemarks { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime SubmittedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedBy { get; set; }

    public Guid? EnrollmentId { get; set; }

    public ICollection<TeacherStudentRequestSchedule> Schedules { get; set; }
        = new List<TeacherStudentRequestSchedule>();
}
