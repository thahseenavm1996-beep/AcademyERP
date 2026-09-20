using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.ReportUnlockRequests;

public class ReportUnlockRequestResponse
{
    public Guid Id { get; set; }
    public Guid ScheduledClassId { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateOnly ReportDate { get; set; }
    public int StudentCount { get; set; }

public int CourseCount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportUnlockRequestStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? AdminRemarks { get; set; }
}
