using AcademyERP.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Admin.Models.ClassReports;

public class CreateClassReportRequest
{
    [Required]
    public Guid TeacherId { get; set; }
    
public Guid ScheduledClassId { get; set; }
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid EnrollmentId { get; set; }
    [Required]
    public DateTime ReportDate { get; set; } = DateTime.Today;

    [Required]
    public AttendanceStatus AttendanceStatus { get; set; }

    [Required]
    public PerformanceRating PerformanceRating { get; set; }

    [Required]
    public HomeworkStatus HomeworkStatus { get; set; }

    [Required]
    public BehaviourRating BehaviourRating { get; set; }

    [Required]
    public ClassOutcome ClassOutcome { get; set; }

    [Required]
    [MaxLength(500)]
    public string LessonTaken { get; set; } = string.Empty;

    [MaxLength(500)]
    public string NextHomework { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? TeacherRemarks { get; set; }

    [Range(1, 300)]
    public int ActualDurationMinutes { get; set; }
}