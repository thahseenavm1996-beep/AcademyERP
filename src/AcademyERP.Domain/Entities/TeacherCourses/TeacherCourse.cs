using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.Teachers;

namespace AcademyERP.Domain.Entities.TeacherCourses;

public class TeacherCourse : BaseEntity
{
    public Guid TeacherId { get; set; }

    public Guid CourseId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public Course Course { get; set; } = null!;
}