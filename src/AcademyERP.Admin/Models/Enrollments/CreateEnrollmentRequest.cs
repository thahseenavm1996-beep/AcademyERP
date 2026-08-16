namespace AcademyERP.Admin.Models.Enrollments;

public class CreateEnrollmentRequest
{
    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid? TeacherId { get; set; }


    // New: multiple weekly class schedules
    


    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime? EndDate { get; set; }


    public decimal MonthlyFee { get; set; }

    public decimal ScholarshipAmount { get; set; }

    public string? ScholarshipReason { get; set; }


    public decimal DiscountAmount { get; set; }

    public string? DiscountReason { get; set; }


    public string? Remarks { get; set; }
    public List<EnrollmentScheduleRequest> Schedules { get; set; }
    = new();
}