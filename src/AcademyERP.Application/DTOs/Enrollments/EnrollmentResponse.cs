using AcademyERP.Domain.Enums;
using AcademyERP.Application.DTOs.EnrollmentSchedules;

namespace AcademyERP.Application.DTOs.Enrollments;

public class EnrollmentResponse
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public Guid CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public Guid? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal ScholarshipAmount { get; set; }

    public string? ScholarshipReason { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? DiscountReason { get; set; }

    public decimal FinalMonthlyFee { get; set; }

    public DateTime NextBillingDate { get; set; }

    public EnrollmentStatus Status { get; set; }

    public string? Remarks { get; set; }
    public List<EnrollmentScheduleResponse> Schedules { get; set; }
    = new();
}