using AcademyERP.Application.DTOs.EnrollmentSchedules;

namespace AcademyERP.Application.DTOs.TeacherStudentRequests;

public class CreateTeacherStudentRequestDto
{
    public Guid? StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string StudentPhoneNumber { get; set; } = string.Empty;

    public Guid CourseId { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Today;

    public decimal? StudentFee { get; set; }

    public string? TeacherRemarks { get; set; }

    public List<CreateEnrollmentScheduleRequest> Schedules { get; set; }
        = new();
}
