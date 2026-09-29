using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.TeacherStudentRequests;

public class TeacherStudentRequestResponse
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public Guid? StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentPhoneNumber { get; set; } = string.Empty;
    public string? StudentAdmissionNumber { get; set; }

    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;

    public TeacherStudentRequestStatus Status { get; set; }

    public decimal? StudentFee { get; set; }
    public decimal? TeacherSalary { get; set; }

    public string? TeacherRemarks { get; set; }
    public string? AdminRemarks { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Guid? EnrollmentId { get; set; }

    public List<TeacherStudentRequestScheduleResponse> Schedules { get; set; }
        = new();
}
