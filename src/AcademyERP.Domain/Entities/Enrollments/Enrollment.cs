using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;


namespace AcademyERP.Domain.Entities.Enrollments;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }
    public Guid? TeacherId { get; set; }

    public Guid TimeSlotId { get; set; }

    public Guid ClassDurationId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal ScholarshipAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalMonthlyFee { get; set; }

    public DateTime NextBillingDate { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    // Navigation Properties

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public Teacher? Teacher { get; set; }

    public TimeSlot TimeSlot { get; set; } = null!;

    public ClassDuration ClassDuration { get; set; } = null!;
    public string? ScholarshipReason { get; set; }

    public string? DiscountReason { get; set; }

    public string? Remarks { get; set; }
}