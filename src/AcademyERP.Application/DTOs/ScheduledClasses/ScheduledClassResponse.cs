namespace AcademyERP.Application.DTOs.ScheduledClasses;

public class ScheduledClassResponse
{
    public Guid Id { get; set; }

    public Guid TeachingScheduleId { get; set; }

    public DateOnly ClassDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }


    public string StudentName { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;


    public string Status { get; set; } = string.Empty;


    public DateTime? ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }


    public string? CancellationReason { get; set; }

    public string? Remarks { get; set; }


    // For future attendance integration
    public string AttendanceStatus { get; set; } = "Not Marked";


    // For future class report integration
    public string ClassReportStatus { get; set; } = "Pending";
}