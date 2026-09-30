using AcademyERP.Application.DTOs.EnrollmentSchedules;
namespace AcademyERP.Application.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid? TeacherId { get; set; }

    // When true, EnrollmentService may automatically add the
    // requested Day + TimeSlot to the teacher's availability.
    // Used by Teacher Student Request approval.
    public bool AutoCreateTeacherAvailability { get; set; }

    public List<CreateEnrollmentScheduleRequest> Schedules { get; set; }
        = new();


    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime? EndDate { get; set; }


    public decimal MonthlyFee { get; set; }

    public decimal TeacherPaymentAmount { get; set; }


    public decimal ScholarshipAmount { get; set; }

    public string? ScholarshipReason { get; set; }


    public decimal DiscountAmount { get; set; }

    public string? DiscountReason { get; set; }


    public string? Remarks { get; set; }
}