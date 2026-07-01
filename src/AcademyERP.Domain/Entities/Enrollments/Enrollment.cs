using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Enrollments;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; set; }

    public Guid ProgramId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal ScholarshipAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalMonthlyFee { get; set; }

    public DateTime NextBillingDate { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public string? Remarks { get; set; }
}