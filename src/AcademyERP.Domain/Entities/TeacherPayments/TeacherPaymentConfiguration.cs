using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Teachers;

namespace AcademyERP.Domain.Entities.TeacherPayments;

public class TeacherPaymentConfiguration : BaseEntity
{
    public Guid TeacherId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public decimal PaymentAmount { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public string? Remarks { get; set; }
    public ICollection<TeacherPaymentPause> Pauses { get; set; }
    = new List<TeacherPaymentPause>();
}
