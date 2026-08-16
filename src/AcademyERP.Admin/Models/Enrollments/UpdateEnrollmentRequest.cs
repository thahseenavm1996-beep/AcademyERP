using AcademyERP.Domain.Enums;


namespace AcademyERP.Admin.Models.Enrollments;

public class UpdateEnrollmentRequest
{
    public Guid? TeacherId { get; set; }

    public Guid TimeSlotId { get; set; }

    public Guid ClassDurationId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal ScholarshipAmount { get; set; }

    public string? ScholarshipReason { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? DiscountReason { get; set; }

    public EnrollmentStatus Status { get; set; }

    public string? Remarks { get; set; }

    public List<EnrollmentScheduleRequest> Schedules { get; set; }
    = new();
}