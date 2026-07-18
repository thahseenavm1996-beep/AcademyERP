using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.TeacherCourses;
using AcademyERP.Domain.Entities.Programs;

namespace AcademyERP.Domain.Entities.Courses;

public class Course : BaseEntity
{
    public string CourseCode { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal StandardMonthlyFee { get; set; }

    public bool IsGroupClassAllowed { get; set; }

    public bool IsActive { get; set; } = true;
    public Guid ProgramId { get; set; }

    public Program Program { get; set; } = null!;

    // Navigation Properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
}