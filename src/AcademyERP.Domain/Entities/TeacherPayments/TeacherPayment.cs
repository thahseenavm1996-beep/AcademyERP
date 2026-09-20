using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.TeacherPayments;

public class TeacherPayment : BaseEntity
{
    public Guid TeacherId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public DateTime CycleStartDate { get; set; }

    public DateTime CycleEndDate { get; set; }

    public DateTime PayableFromDate { get; set; }

public DateTime PayableToDate { get; set; }

public int PayableDays { get; set; }


public int CycleScheduledDays { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public TeacherPaymentStatus Status { get; set; } =
        TeacherPaymentStatus.Pending;

    public DateTime? PaidDate { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }
}
